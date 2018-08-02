using POInvoice.Data.Domain;
using POInvoice.Data.Persistence;
using POInvoice.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace POInvoice.Controllers.APIs
{
    public class UploadSupportingDocController : ApiController
    {
        private PoInvoice _context;

        public UploadSupportingDocController()
        {
            _context = new PoInvoice();
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddDoc(int id)
        {
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            await AddDocumentToFileSystemAndDb(id, provider);

            return Ok();
        }

        private async Task AddDocumentToFileSystemAndDb(int id, MultipartMemoryStreamProvider provider)
        {
            for (int i = 0; i < provider.Contents.Count - 1; i++)
            {
                var file = provider.Contents[i];

                if (file.Headers.ContentDisposition.FileName != null)
                {
                    var dir = FilePaths.WriteSupportingDoc + FilePaths.FolderFromUniqueId(id);
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);

                    // For an unknown reason, the FileName has an extra '\' everywhere, that's why it's replaced with string.Empty
                    var writePath = dir + file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);

                    var dataStream = await file.ReadAsByteArrayAsync();
                    using (var saveFile = File.OpenWrite(writePath))
                    {
                        await saveFile.WriteAsync(dataStream, 0, dataStream.Length);
                    }

                    var dateModified = await FileDates.StreamToString(provider.Contents[i + 1]);
                    await AddFileReferenceToDatabase(id, dateModified, writePath);
                }
            }
        }

        private async Task AddFileReferenceToDatabase(int id, DateTime dateModified, string writePath)
        {
            // For any given type of file that the user might upload as a supporting document,
            // we can read in the dateModified property of the file.
            // We CANNOT read in the dateCreated (it is supported in IE but no other browsers)
            // So, we just assume the dateCreated/dateModified are the same.

            var document = new Document
            {
                PoFormId = id,
                TypeOfDocument = Data.Services.TypeOfDocument.Supporting,
                FilePath = writePath,
                Created = dateModified,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                Modified = dateModified,
                ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name
            };

            _context.Documents.Add(document);
            
            await _context.SaveChangesAsync();
        }
    }
}
