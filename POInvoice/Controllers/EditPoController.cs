using POInvoice.Data.Persistence;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace POInvoice.Controllers
{
    public class EditPoController : Controller
    {
        private PoInvoice _context;

        public EditPoController()
        {
            _context = new PoInvoice();
        }

        public ActionResult Index(int id)
        {
            var poForm = _context.PoForms
                .Include(p => p.Vendor)
                .Include(p => p.LineItems)
                .FirstOrDefault(p => p.Id == id);

            if (poForm == null)
                return HttpNotFound("Could not find that PO Form to edit.");

            return View("FillablePo", poForm);
        }
    }
}