using System.Web.Mvc;
using eMotive.CMS.Managers.Interfaces;

namespace eMotive.CMS.Areas.Admin.Controllers
{
    [RouteArea("Admin"), RoutePrefix("Navigation"), Route("{action=Menu}")]
    public class NavigationController : Controller
    {
        private readonly INavigationManager _navigationManager;

        public NavigationController(INavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

       // [ChildActionOnly]
        public PartialViewResult Menu()
        {
            return PartialView("_AdminNav", _navigationManager.GetTestMenu());
        }


	}
}