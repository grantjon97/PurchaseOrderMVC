using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using POInvoice.Data.Domain;
using POInvoice.Data.Services;
using POInvoice.ViewModels;

namespace POInvoice.Services
{
    public static class PoFormConversions
    {
        private static readonly POInvoice.Data.Persistence.PoInvoice _context;

        static PoFormConversions()
        {
            _context = new Data.Persistence.PoInvoice();
        }

        public static string FormatUserName(string username)
        {
            return username.Replace(@"WBP\", string.Empty);
        }

        public static string FormatDollar(decimal amount)
        {
            return String.Format("{0:C}", amount);
        }

        public static Document CreatePoDocument(PoForm poForm)
        {
            return new Document
            {
                PoFormId = poForm.Id,
                TypeOfDocument = TypeOfDocument.Po,
                FilePath = FilePaths.WritePoForm + poForm.Id + ".pdf",
                Created = DateTime.Now,
                CreatedBy = HttpContext.Current.User.Identity.Name,
                Modified = DateTime.Now,
                ModifiedBy = HttpContext.Current.User.Identity.Name
            };
        }

        public static void EditForm(PoForm poFormInDb, PoForm poForm)
        {
            poFormInDb.Overview = poForm.Overview;
            poFormInDb.Attn = poForm.Attn;
            poFormInDb.DateRequired = poForm.DateRequired;
            poFormInDb.RequisitioningDepartment = poForm.RequisitioningDepartment;
            poFormInDb.ShipViaTerms = poForm.ShipViaTerms;
            poFormInDb.Vendor = poForm.Vendor;
            poFormInDb.PoNumber = poForm.PoNumber;

            // LineItems must be cleared first so that the DB doesn't just add additional ones.
            // Clear out line items, save changes, then add all of them again as new ones.
            poFormInDb.LineItems = new List<LineItem>();

            //poFormInDb.GrandTotal = poForm.GrandTotal;
            poFormInDb.Modified = DateTime.Now;
            poFormInDb.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
        }

        public static PoForm MakeDuplicateForm(PoForm poForm, string newOverview)
        {
            return new PoForm
            {
                //Username = FormatUserName(HttpContext.Current.User.Identity.Name),
                //FilePath = FilePaths.WritePoForm + myGuid + ".pdf",
                Overview = newOverview,
                //DateLastModified = DateTime.Now,

                // Blanket PO copies MUST be re-approved, get a new PO number, etc.
                Status = Data.Services.Status.Pending,

                RequisitioningDepartment = poForm.RequisitioningDepartment,
                DateRequired = poForm.DateRequired,
                ShipViaTerms = poForm.ShipViaTerms ?? "",
                Attn = poForm.Attn
            };
        }

        public static PoFormSearchData PoFormToPoFormSearchData(PoForm poForm)
        {
            return new PoFormSearchData
            {
                Id = poForm.Id,
                Overview = poForm.Overview,

                Modified = poForm.Modified,
                ModifiedBy = poForm.ModifiedBy,

                Status = poForm.Status,
                VendorName = _context.Vendors.FirstOrDefault(v => v.Id == poForm.VendorId).Name,
                PoNumber = poForm.PoNumber,
                GrandTotal = CalculateGrandTotal(poForm)
            };
        }

        public static decimal CalculateGrandTotal(PoForm poForm)
        {
            decimal grandTotal = 0.0m;

            foreach (var lineItem in poForm.LineItems)
                grandTotal += (lineItem.Quantity * lineItem.UnitCost);

            return grandTotal;
        }
    }
}