using System.Web.Mvc;
using System.Web.Routing;

namespace eMotive.CMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("api/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" }); 

            routes.MapMvcAttributeRoutes();
        }
    }
}
