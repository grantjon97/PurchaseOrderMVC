using System.Collections.Generic;

namespace POInvoice.Data.Domain
{
    public class Vendor
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public List<PoForm> PoForms { get; set; } = new List<PoForm>();
    }
}
