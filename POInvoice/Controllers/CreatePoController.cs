using POInvoice.Data.Domain;
using System.Collections.Generic;
using System.Web.Mvc;

namespace POInvoice.Controllers
{
    public class CreatePoController : Controller
    {
        // GET: CreatePo
        public ActionResult Index()
        {
            return View("FillablePo", new PoForm { LineItems = new List<LineItem> { new LineItem() },
                                                   Vendor = new Vendor() } );
        }
    }
}