using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eMotive.CMS.Services.Interfaces;

namespace eMotive.CMS.Core.Modules
{
    public class RequestLogModule : IHttpModule
    {
        private readonly IAuditService _logService;

        public RequestLogModule(IAuditService logService)
        {
            _logService = logService;
        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += LogRequest;
        }

        protected void LogRequest(object sender, EventArgs e)
        {
            var htmlApp = (HttpApplication)sender;

            var username = "Guest";

            if (htmlApp.Context.User != null)
                if (!string.IsNullOrEmpty(htmlApp.Context.User.Identity.Name))
                    username = htmlApp.Context.User.Identity.Name;

            var controllerName = htmlApp.Request.RequestContext.RouteData.Values["Controller"].ToString();
            var actionName = htmlApp.Request.RequestContext.RouteData.Values["Action"].ToString();

          //  _logService.LogRequest(username, actionName, controllerName);
        }

        public void Dispose()
        {
            //do we need to do anything here?
        }
    }
}