using POInvoice.Data.Domain;
using POInvoice.Data.Persistence;
using POInvoice.Services;
using POInvoice.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Mvc;
using TuesPechkin.Service;

namespace POInvoice.Controllers
{
    public class HomeController : Controller
    {
        //protected override string UserName => User.Identity.Name;
        private PoInvoice _context;

        public HomeController()
        {
            _context = new PoInvoice();
        }
        
        public ActionResult Index(string yearSelected)
        {
            bool result = Int32.TryParse(yearSelected, out int year);

            // Loading the home page (without a query string) should result in
            // viewing the current year's PO forms.
            if (result == false)
                year = DateTime.Today.Year;

            ViewBag.Year = year;

            var forms = _context.PoForms
                                .Include(p => p.LineItems)
                                .Where(p => p.Modified.Year == year)
                                .ToList();

            var viewForm = new List<PoFormSearchData>();
            
            foreach (var form in forms)
                viewForm.Add(PoFormConversions.PoFormToPoFormSearchData(form));

            return View(viewForm);
        }

        [HttpPost]
        public async Task<ActionResult> Save(PoForm poForm)
        {
            if (poForm == null || poForm.Vendor == null || poForm.LineItems == null)
                throw new InvalidDataException();

            if (poForm.Id == 0)
                SaveNewPoForm(poForm);
            else
                SaveEditedPoForm(poForm);

            // Take the PO form and turn it into a PDF, then write it.
            var url = Url.Action("CompleteForm", "Home", new { id = poForm.Id }, protocol: Request.Url.Scheme);
            var pdf = TuesPechkinService.ConvertToPdf(TuesPechkin.Service.Uri.PrdApp01, url);
            if (pdf == null) return HttpNotFound();

            await Pdf.WriteAsPdf(poForm.Id, pdf);

            return RedirectToAction("Info/" + poForm.Id, "Home");
        }

        private void SaveNewPoForm(PoForm poForm)
        {
            poForm.Created = DateTime.Now;
            poForm.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            poForm.Modified = DateTime.Now;
            poForm.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            poForm.SubmittedDate = DateTime.Now;

            foreach (var lineItem in poForm.LineItems)
            {
                lineItem.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                lineItem.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                lineItem.Created = DateTime.Now;
                lineItem.Modified = DateTime.Now;
            }

            // These arguments may seem redundant, but this function was created to handle
            // looking at any random vendor that could have come from anywhere
            AssignVendorToPoForm(poForm, poForm.Vendor);

            _context.PoForms.Add(poForm);
            _context.SaveChanges();

            poForm.Documents.Add(PoFormConversions.CreatePoDocument(poForm));
            _context.SaveChanges();
        }

        private void SaveEditedPoForm(PoForm poForm)
        {
            var poFormInDb = _context.PoForms
                                .Include(p => p.Vendor)
                                .Include(p => p.LineItems)
                                .First(p => p.Id == poForm.Id);

            poFormInDb.Modified = DateTime.Now;
            poFormInDb.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            poFormInDb.ExpirationDate = poForm.ExpirationDate;
            poFormInDb.ResponsibleParty = poForm.ResponsibleParty;
            poFormInDb.PoNumber = poForm.PoNumber;

            poFormInDb.Overview = poForm.Overview;
            poFormInDb.Status = Data.Services.Status.Pending;
            poFormInDb.Attn = poForm.Attn;
            poFormInDb.DateRequired = poForm.DateRequired;
            poFormInDb.RequisitioningDepartment = poForm.RequisitioningDepartment;
            poFormInDb.ShipViaTerms = poForm.ShipViaTerms;

            AssignVendorToPoForm(poFormInDb, poForm.Vendor);

            _context.SaveChanges();


            var oldLineItems = poFormInDb.LineItems;
            ClearLineItems(poFormInDb);

            poFormInDb.LineItems = poForm.LineItems;

            CheckChangesInLineItems(oldLineItems, poFormInDb.LineItems);

            _context.SaveChanges();
        }

        private void ClearLineItems(PoForm poForm)
        {
            // We have to actually get rid of the line items in the table so that they don't
            // have a null PoFormId
            var lineItems = _context.LineItems.Where(l => l.PoFormId == poForm.Id).ToList();
            _context.LineItems.RemoveRange(lineItems);

            poForm.LineItems.Clear();

            _context.SaveChanges();
        }

        private void CheckChangesInLineItems(List<LineItem> oldLineItems, List<LineItem> newLineItems)
        {
            foreach (var newLineItem in newLineItems)
            {
                var oldLineItem = oldLineItems.FirstOrDefault(l => l.Id == newLineItem.Id);
                
                if (oldLineItem != null)
                {
                    // Here, we know that newLineItem was either an edit, or left alone, from a previous one.
                    newLineItem.Created = oldLineItem.Created;
                    newLineItem.CreatedBy = oldLineItem.CreatedBy;

                    if (LineItemsAreEqual(oldLineItem, newLineItem))
                    {
                        newLineItem.Modified = oldLineItem.Modified;
                        newLineItem.ModifiedBy = oldLineItem.ModifiedBy;
                    }
                    else
                    {
                        newLineItem.Modified = DateTime.Now;
                        newLineItem.ModifiedBy = HttpContext.User.Identity.Name;
                    }
                }
                else
                {
                    // Here, we know that the lineItem is a brand new one.
                    newLineItem.CreatedBy = HttpContext.User.Identity.Name;
                    newLineItem.ModifiedBy = HttpContext.User.Identity.Name;
                }
            }
        }

        private bool LineItemsAreEqual(LineItem firstLineItem, LineItem secondLineItem)
        {
            return ((firstLineItem.AccountNumber == null && secondLineItem.AccountNumber == null) || (firstLineItem.AccountNumber.Equals(secondLineItem.AccountNumber))) &&
                ((firstLineItem.Description == null && secondLineItem.Description == null) || (firstLineItem.Description.Equals(secondLineItem.Description))) &&
                (firstLineItem.Quantity == secondLineItem.Quantity) &&
                (firstLineItem.UnitCost == secondLineItem.UnitCost);
        }

        private void AssignVendorToPoForm(PoForm poFormInDb, Vendor vendor)
        {
            bool vendorAlreadyExists = false;
            int i = 0;
            var vendors = _context.Vendors.ToList();

            while (!vendorAlreadyExists && i < vendors.Count)
            {
                // NOTE: We are only checking if the user
                // has entered the same vendor's NAME, and if they did,
                // then we match it to an existing vendor, regardless of if the rest of the data
                // is different (it shouldn't be different, assuming one vendor has one address, city, state, and zip)
                // Also, "examPleVenDOR" will be equal to "EXAmpleVendoR"
                if (vendor.Name != null &&
                    vendor.Name.ToLower().Equals(vendors[i].Name.ToLower()))
                {
                    poFormInDb.Vendor = vendors[i];
                    poFormInDb.VendorId = vendors[i].Id;
                    vendorAlreadyExists = true;
                }

                i++;
            }

            // Ensure that a new vendor has the PO form assigned to it.
            if (!vendorAlreadyExists)
            {
                poFormInDb.Vendor = vendor;
            }
        }

        public async Task<ActionResult> WritePoFormToFile(string strId)
        {
            var id = Convert.ToInt32(strId);

            var url = Url.Action("CompleteForm", "Home", new { id }, protocol: Request.Url.Scheme);
            var pdf = TuesPechkinService.ConvertToPdf(TuesPechkin.Service.Uri.PrdApp01, url);
            if (pdf == null) return HttpNotFound();

            await Pdf.WriteAsPdf(id, pdf);

            return RedirectToAction("Info/" + id, "Home");
        }

        [AllowAnonymous]
        public ActionResult CompleteForm(int id)
        {
            var poForm = _context.PoForms
                .Include(p => p.Vendor)
                .Include(p => p.LineItems)
                .FirstOrDefault(p => p.Id == id);

            if (poForm == null)
                return HttpNotFound();

            return View(poForm);
        }

        public ActionResult ViewPdf(string filePath)
        {
            var pdf = System.IO.File.ReadAllBytes(filePath);
            var extension = Path.GetExtension(filePath);

            // View PDFs, but download every other type of file.
            if (extension.Equals(".pdf"))
                return File(pdf, MediaTypeNames.Application.Pdf);
            else
                return File(pdf, MediaTypeNames.Application.Octet, Path.GetFileName(filePath));
        }

        public ActionResult Info(int id)
        {
            var poForm = _context.PoForms
                .Include(p => p.Vendor)
                .Include(p => p.LineItems)
                .Include(p => p.Documents)
                .FirstOrDefault(p => p.Id == id);

            if (poForm == null)
                return HttpNotFound("This PO form no longer exists.");

            return View(poForm);
        }
    }
}