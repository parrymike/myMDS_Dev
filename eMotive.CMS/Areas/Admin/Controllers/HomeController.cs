using System.Web.Mvc;

namespace eMotive.CMS.Areas.Admin.Controllers
{
    [RouteArea("Admin"), RoutePrefix("Home"), Route("{action=Index}")]
    public class HomeController : Controller
    {
        //
        // GET: /Admin/Home/
        public ActionResult Index()
        {
            return View();
        }
	}
}