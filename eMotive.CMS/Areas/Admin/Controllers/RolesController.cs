using System.Web.Mvc;

namespace eMotive.CMS.Areas.Admin.Controllers
{
    [RouteArea("Admin"), RoutePrefix("Roles"), Route("{action=Index}")]
    public class RolesController : Controller
    {
        //
        // GET: /Admin/Roles/
        public ActionResult Index()
        {
            return View();
        }
	}
}