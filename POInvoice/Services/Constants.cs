using System.Web;

namespace POInvoice.Services
{
    public static class Contants
    {
        public static string CurrentUser = HttpContext.Current.User.Identity.Name;
    }
}