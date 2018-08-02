namespace POInvoice.Data.Domain
{
    public class LineItem : CreatedAndModified
    {
        public int Id { get; set; }

        public PoForm PoForm { get; set; }
        public int PoFormId { get; set; }

        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        //public decimal Total { get; set; }
    }
}
