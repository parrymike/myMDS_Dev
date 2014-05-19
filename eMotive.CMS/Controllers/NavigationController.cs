using System.Web.Mvc;
using eMotive.CMS.Managers.Interfaces;

namespace eMotive.CMS.Controllers
{
    public class NavigationController : Controller
    {
        private readonly INavigationManager _navigationManager;

        public NavigationController(INavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public PartialViewResult MainMenu()
        {
            return PartialView("_AdminNav", _navigationManager.GetTestMenu());
        }

	}
}