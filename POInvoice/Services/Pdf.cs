using System;
using System.Linq;
using POInvoice.Data.Persistence;
using System.Threading.Tasks;

namespace POInvoice.Services
{
    public static class Pdf
    {
        private static readonly PoInvoice _context;

        static Pdf()
        {
            _context = new PoInvoice();
        }

        public static async Task WriteAsPdf(int id, byte[] pdf)
        {

            var filePath = _context.Documents.FirstOrDefault(d => d.PoFormId == id).FilePath;
            try
            {
                using (var saveFile = System.IO.File.OpenWrite(filePath))
                {
                    await saveFile.WriteAsync(pdf, 0, pdf.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Oops! The file could not be saved to the filesystem.\n{ex.Message}", ex);
            }
        }
    }
}