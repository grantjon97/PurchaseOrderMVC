using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Threading.Tasks;
using POInvoice.Data.Domain;
using POInvoice.Data.Persistence;
using POInvoice.Services;

namespace POInvoice.Controllers.APIs
{
    public class UploadController : ApiController
    {
        private PoInvoice _context;

        public UploadController()
        {
            _context = new PoInvoice();
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddOldPoForms(decimal total, string accountNumber = null,
                                                            string vendorName = null, string poNumber = null, string responsibleParty = null,
                                                            DateTime? submitDate = null, DateTime? expireDate = null)
        {
            // NOTE: This method accounts for submitting multiple po forms at the same time,
            //       given that the user has entered all of the above arguments for each form.
            //       Currently, since there is so much manual input, only one form is allowed to be
            //       submitted at a time.

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            // "provider.Contents.Count - 1" just saves one unneeded iteration (would work without -1)
            for (int i = 0; i < provider.Contents.Count - 1; i++)
            {
                var file = provider.Contents[i];

                if (file.Headers.ContentDisposition.FileName != null)
                {
                    var fileName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    var dateModified = await FileDates.StreamToString(provider.Contents[i + 1]);

                    var dataArguments = new Dictionary<string, string>
                    {
                        {"fileName", fileName },
                        {"total", total.ToString() },
                        {"accountNumber", accountNumber },
                        {"vendorName", vendorName },
                        {"poNumber", poNumber  },
                        {"responsibleParty", responsibleParty },
                    };

                    var newPoForm = SaveOldPoToDatabase(dataArguments, dateModified, submitDate, expireDate);

                    await SaveOldPoToFileSystem(file, newPoForm.Id);
                }
            }

            return Ok();
        }

        private PoForm SaveOldPoToDatabase(Dictionary<string, string> data, DateTime dateModified, DateTime? dateSubmitted, DateTime? dateExpired)
        {
            var vendorName = data["vendorName"];
            var vendor = _context.Vendors.FirstOrDefault(v => v.Name.Equals(vendorName)) ?? new Vendor { Name = data["vendorName"] };

            var poForm = new PoForm
            {
                Overview = Path.GetFileNameWithoutExtension(data["fileName"]),
                Status = data["poNumber"] != null ? Data.Services.Status.Approved : Data.Services.Status.Pending,
                Vendor = vendor,
                VendorId = vendor.Id,
                PoNumber = data["poNumber"] ?? null,

                ShipViaTerms = "Net 30",

                Created = dateModified,
                CreatedBy = data["responsibleParty"] ?? Contants.CurrentUser,
                Modified = dateModified,
                ModifiedBy = data["responsibleParty"] ?? Contants.CurrentUser,
                ResponsibleParty = data["responsibleParty"] ?? Contants.CurrentUser,

                SubmittedDate = dateSubmitted,
                ExpirationDate = dateExpired
            };

            _context.PoForms.Add(poForm);
            _context.SaveChanges();

            // We have to apply these changes after saving the poForm because
            // before it is saved, the id is 0.
            poForm.LineItems = new List<LineItem> { new LineItem { PoFormId = poForm.Id,
                                                                   AccountNumber = data["accountNumber"],
                                                                   Description = Path.GetFileNameWithoutExtension(data["fileName"]),
                                                                   Quantity = 1,
                                                                   UnitCost = Convert.ToDecimal(data["total"]),
                                                                   Created = dateModified,
                                                                   Modified = dateModified } };

            poForm.Documents = new List<Document> { new Document { PoFormId = poForm.Id,
                                                                   TypeOfDocument = Data.Services.TypeOfDocument.Po,
                                                                   FilePath = FilePaths.WritePoForm + poForm.Id + ".pdf",
                                                                   Created = dateModified,
                                                                   Modified = dateModified } };

            _context.SaveChanges();

            return poForm;
        }

        private async Task SaveOldPoToFileSystem(HttpContent file, int id)
        {
            // Save the file at: (subject to change)
            // D:\TempData\PoFormTests\{id}.pdf
            var writePath = FilePaths.WritePoForm + id + ".pdf";

            // Write the file in that directory
            var dataStream = await file.ReadAsByteArrayAsync();

            using (var saveFile = File.OpenWrite(writePath))
            {
                await saveFile.WriteAsync(dataStream, 0, dataStream.Length);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddPdfToPendingForm(string strId, string poNumber, string numberOfFiles)
        {
            var id = Convert.ToInt32(strId);

            var poForm = _context.PoForms.FirstOrDefault(p => p.Id == id);

            poForm.PoNumber = poNumber;
            poForm.Status = Data.Services.Status.Approved;
            poForm.FinalizedDate = DateTime.Now;
            _context.SaveChanges();

            // If the user does not enter a file to be uploaded with the PO number,
            // we want to just create an electronic PDF of the approved form.
            if (Convert.ToInt32(numberOfFiles) > 0)
                await AddPdfToFileSystem(id);

            return Ok();
        }


        /*
         *   Credit to much of the code in AddPdfToFileSystem(...) goes to 
         *   
         *   Nehemiah Langness, CPS.  
         *   
         *   Much of this method was originally part of his work, which can be found in:
         *   CPS.FileTransferService -> FileTransferServiceDll -> ViewLayer -> Apis -> FileConfigurationsController.cs
         *   and
         *   CPS.FileTransferService -> Views -> FileConfigurations -> _FileUpload.cshtml
         */
        private async Task AddPdfToFileSystem(int id)
        {
            var writePath = FilePaths.WritePoForm + id + ".pdf";

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var file in provider.Contents)
            {
                var dataStream = await file.ReadAsByteArrayAsync();
                using (var saveFile = File.OpenWrite(writePath))
                {
                    await saveFile.WriteAsync(dataStream, 0, dataStream.Length);
                }
            }
        }
    }
}
