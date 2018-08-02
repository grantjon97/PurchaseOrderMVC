using POInvoice.Properties;

namespace POInvoice.Services
{
    public static class FilePaths
    {
        public static string WriteSupportingDoc => Settings.Default.Test ? TestWriteSupportingDoc : ProdWriteSupportingDoc;
        public static string WritePoForm => Settings.Default.Test ? TestWritePoForm : ProdWritePoForm;


        public const string ProdWriteSupportingDoc = @"D:\STANDALONE\DataFiles\PoForms\PoFormSupportingDocs\";

        public const string ProdWritePoForm = @"D:\STANDALONE\DataFiles\PoForms\PoForms\";

        public const string TestWriteSupportingDoc = @"D:\TempData\PoFormSupportingDocs\";

        public const string TestWritePoForm = @"D:\TempData\PoFormTests\";

        public static string FolderFromUniqueId(int id)
        {
            return id.ToString() + @"\";
        }
    }
}