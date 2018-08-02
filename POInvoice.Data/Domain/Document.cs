using POInvoice.Data.Services;

namespace POInvoice.Data.Domain
{
    public class Document : CreatedAndModified
    {
        public int Id { get; set; }

        public PoForm PoForm { get; set; }
        public int PoFormId { get; set; }

        public TypeOfDocument TypeOfDocument { get; set; }

        public string FilePath { get; set; }

    }
}
