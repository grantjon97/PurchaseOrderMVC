using POInvoice.Data.Persistence;
using System.Linq;
using System.Web.Http;

namespace POInvoice.Controllers.APIs
{
    public class VendorsController : ApiController
    {
        private PoInvoice _context;

        public VendorsController()
        {
            _context = new PoInvoice();
        }

        // GET: Vendor
        public IHttpActionResult GetVendors(string query = null)
        {
            var vendorsQuery = _context.Vendors
                .Where(v => v.Name.Contains(query))
                .ToList();

            //if (String.IsNullOrWhiteSpace(query))
            //    vendorsQuery = vendorsQuery.Where(v => v.Name.Contains(query)).ToList();

            return Ok(vendorsQuery);
        }
    }
}