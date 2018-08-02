using System;
using POInvoice.Data.Services;

namespace POInvoice.ViewModels
{
    public class PoFormSearchData
    {
        public int Id { get; set; }

        public string Overview { get; set; }

        public string VendorName { get; set; }

        public DateTime Modified { get; set; }

        public string ModifiedBy { get; set; }

        public Status Status { get; set; }

        public string PoNumber { get; set; }

        public decimal GrandTotal { get; set; }
    }
}