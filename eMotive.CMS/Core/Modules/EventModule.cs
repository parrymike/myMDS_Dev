using System;
using System.Web;
using eMotive.CMS.Services.Interfaces;
using ServiceStack.WebHost.Endpoints;
//todo: check ths only fires once, if more than once, see how we can stop it occuring again! perhaps clear event list? Perhaps
namespace eMotive.CMS.Core.Modules
{
    public class EventModule : IHttpModule
    {
        private IEventManagerService _eventManagerService;

        public void Init(HttpApplication context)
        {
            context.EndRequest += LogRequest;
        }

        private void LogRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;

            if (app == null || app.Context.Handler == null) return;

           // if (app.Context.Handler is System.Web.Mvc.MvcHandler)
            //{
                _eventManagerService = AppHostBase.Instance.TryResolve<IEventManagerService>();

                if(_eventManagerService != null)
                    _eventManagerService.FireEvents();

 


           // }
        }

        public void Dispose()
        {
            //do we need to do anything here?
        }
    }
}