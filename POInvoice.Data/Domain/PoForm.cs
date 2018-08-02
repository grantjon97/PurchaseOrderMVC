using POInvoice.Data.Services;
using System;
using System.Collections.Generic;

namespace POInvoice.Data.Domain
{
    public class PoForm : CreatedAndModified
    {
        public int Id { get; set; }
        public string Overview { get; set; }
        public string PoNumber { get; set; }

        public Status Status { get; set; }

        public List<Document> Documents { get; set; } = new List<Document>();
        public List<LineItem> LineItems { get; set; }
        public Vendor Vendor { get; set; }
        public int VendorId { get; set; }

        public string ResponsibleParty { get; set; }
        public DateTime? DateRequired { get; set; } = DateTime.Now;
        public string RequisitioningDepartment { get; set; }
        public string ShipViaTerms { get; set; }
        public string Attn { get; set; }

        // Date the PO was sent to the LCMS
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? FinalizedDate { get; set; }
    }
}
