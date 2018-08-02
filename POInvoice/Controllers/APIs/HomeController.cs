using System;
using System.Linq;
using System.Web.Http;
using System.IO;
using System.Threading.Tasks;
using POInvoice.Data.Domain;
using POInvoice.Data.Persistence;
using POInvoice.Services;
using System.Data.Entity;
using TuesPechkin.Service;

namespace POInvoice.Controllers.APIs
{
    public class HomeController : ApiController
    {
        private PoInvoice _context;

        public HomeController()
        {
            _context = new PoInvoice();
        }

        [Route("api/Home/DeleteEntireForm")]
        [HttpDelete]
        public IHttpActionResult DeleteEntireForm(int id)
        {
            var poForm = _context.PoForms.FirstOrDefault(p => p.Id == id);

            if (poForm == null)
                return BadRequest();

            DeleteFiles(id);
            DeleteEntity(id);

            return Ok();
        }

        [Route("api/Home/DeleteSupportingDocument")]
        [HttpDelete]
        public IHttpActionResult DeleteSupportingDocument(int id)
        {
            // Delete from the database
            var document = _context.Documents.FirstOrDefault(d => d.Id == id);

            if (document != null)
            {
                _context.Documents.Remove(document);
                _context.SaveChanges();
            }

            //// Then the filesystem
            if (File.Exists(document.FilePath))
                File.Delete(document.FilePath);

            return Ok();
        }

        [HttpPut]
        public IHttpActionResult RenameForm(int id, string newOverview)
        {
            var poForm = _context.PoForms.FirstOrDefault(p => p.Id == id);

            if (poForm == null)
                return BadRequest("The PO form could not be found.");

            poForm.Overview = newOverview;
            _context.SaveChanges();

            return Ok();
        }

        private void DeleteFiles(int id)
        {
            // Delete all supporting documents and its directory
            if (System.IO.Directory.Exists(FilePaths.WriteSupportingDoc + id))
                Directory.Delete(FilePaths.WriteSupportingDoc + id, true);

            // Delete the PO form PDF
            if (File.Exists(FilePaths.WritePoForm + id + ".pdf"))
                File.Delete(FilePaths.WritePoForm + id + ".pdf");
        }

        private void DeleteEntity(int id)
        {
            var poForm = _context.PoForms.FirstOrDefault(p => p.Id == id);

            _context.PoForms.Remove(poForm);
            _context.SaveChanges();
        }

        [HttpPost]
        public async Task<IHttpActionResult> DuplicatePo(int id, string overview, string username)
        {
            if (FormNameTaken(overview.Trim()))
                return Json(new { success = false, errorMessage = "The name you specified is already in use." });
            
            var newPo = DuplicatePoInDb(id, overview, username);
            var newPoId = newPo.Id;

            await DuplicatePoInFileSystem(id, newPoId);

            return Json(new { success = true, newId = newPoId });
        }

        private PoForm DuplicatePoInDb(int id, string overview, string username)
        {
            var poForm = _context.PoForms
                .AsNoTracking()
                .Include(p => p.Vendor)
                .Include(p => p.LineItems)
                .Include(p => p.Documents)
                .FirstOrDefault(p => p.Id == id);

            poForm.Overview = overview;
            poForm.Status = Data.Services.Status.Pending;
            poForm.PoNumber = null;
            poForm.Vendor = _context.PoForms.FirstOrDefault(p => p.Id == id).Vendor;
            poForm.VendorId = _context.PoForms.FirstOrDefault(p => p.Id == id).VendorId;
            poForm.Created = DateTime.Now;
            poForm.CreatedBy = username;
            poForm.Modified = DateTime.Now;
            poForm.ModifiedBy = username;
            poForm.SubmittedDate = DateTime.Now;

            _context.PoForms.Add(poForm);
            _context.SaveChanges();

            ChangeDuplicateFilePathsInDb(id, poForm.Id);

            return poForm;
        }

        private void ChangeDuplicateFilePathsInDb(int oldId, int newId)
        {
            var documents = _context.Documents.Where(d => d.PoFormId == newId).ToList();

            foreach (var document in documents)
                document.FilePath = document.FilePath.Replace(oldId.ToString(), newId.ToString());

            _context.SaveChanges();
        }

        private async Task DuplicatePoInFileSystem(int oldId, int newId)
        {
            var dir = FilePaths.WriteSupportingDoc + FilePaths.FolderFromUniqueId(newId);
            var supportingDocuments = _context.Documents
                .Where(d => d.PoFormId == oldId)
                .Where(d => d.TypeOfDocument == Data.Services.TypeOfDocument.Supporting)
                .ToList();

            var newPoWritePath = FilePaths.WritePoForm + newId + ".pdf";

            if (!Directory.Exists(dir) && !File.Exists(newPoWritePath))
            {
                Directory.CreateDirectory(dir);
                foreach (var doc in supportingDocuments)
                    File.Copy(doc.FilePath, dir + Path.GetFileName(doc.FilePath));
                    
                await WriteToFileSystem(newId);
            }
        }

        [HttpGet]
        private async Task WriteToFileSystem(int id)
        {
            var url = new System.Uri(Request.RequestUri, RequestContext.VirtualPathRoot) + "Home/CompleteForm/" + id;
            var pdf = TuesPechkinService.ConvertToPdf(TuesPechkin.Service.Uri.PrdApp01, url);
            if (pdf == null) return;

            await Pdf.WriteAsPdf(id, pdf);
        }

        private Boolean FormNameTaken(string name)
        {
            var poForm = _context.PoForms.FirstOrDefault(p => p.Overview.Equals(name));

            return poForm != null;
        }
    }
}
