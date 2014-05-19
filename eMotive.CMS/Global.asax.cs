using System.Web.Mvc;
using System.Web.Routing;
using eMotive.CMS.App_Start;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;


namespace eMotive.CMS
{//https://github.com/ServiceStack/ServiceStack/wiki/Run-servicestack-side-by-side-with-another-web-framework
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            LogManager.LogFactory = new NLogFactory();

            new AppHost().Init();
        }
    }
}
