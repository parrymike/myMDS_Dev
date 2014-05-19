using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eMotive.CMS.Services.Interfaces;
using ServiceStack;
using ServiceStack.WebHost.Endpoints;

namespace eMotive.CMS.Core.Modules
{
    public class ErrorLogTest : IHttpModule
    {
        private IMessageBusService _logService;

        public void Init(HttpApplication context)
        {
            context.EndRequest += LogRequest;
        }

        private void LogRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;

            if (app == null || app.Context.Handler == null) return;

            if (app.Context.Handler is System.Web.Mvc.MvcHandler)
            {
                _logService = AppHostBase.Instance.TryResolve<IMessageBusService>();
            }
        }

        public void Dispose()
        {
            //do we need to do anything here?
        }
    }
}