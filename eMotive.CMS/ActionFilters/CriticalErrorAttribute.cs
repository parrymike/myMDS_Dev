/*using System.Linq;
using System.Web.Mvc;
using eMotive.CMS.Core;
using eMotive.CMS.Extensions;
using eMotive.CMS.Services.Interfaces;
using Ninject;

namespace eMotive.CMS.ActionFilters
{
    public class CriticalErrorAttribute : ActionFilterAttribute
    {
        [Inject]
        public IMessageBusService MessageBusService { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //if the request is an ajax request, we don't want a redirect to happen
            //the controller dealing with the ajax request can fetch the critical
            //errors and pass them back to the user for display
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var criticalErrors = MessageBusService.Fetch();

                if (criticalErrors.HasContent())
                {
                    var helper = new UrlHelper(filterContext.RequestContext);



                    var controller = filterContext.RouteData.DataTokens["controller"] ?? string.Empty;
                    //  var action = filterContext.RouteData.DataTokens["action"] ?? string.Empty;
                    var area = filterContext.RouteData.DataTokens["area"] ?? string.Empty;

                    var url = helper.Action("Index", "Error", new { area = (!string.IsNullOrEmpty(area.ToString()) && area.ToString() == "Admin") ? "Admin" : "" });

                    var errorView = new ErrorView
                    {
                        Referer = string.Format("/{0}/{1}", area, controller),
                        ControllerName = controller.ToString(),
                        Errors = criticalErrors.Where(m => m.IsError).Select(m => m.Details)//TODO: make this more efficient?
                    };

                    filterContext.Controller.TempData["CriticalErrors"] = errorView;

                    filterContext.Result = new RedirectResult(url);
                }
            }
            base.OnActionExecuted(filterContext);
        }

    }
}*/