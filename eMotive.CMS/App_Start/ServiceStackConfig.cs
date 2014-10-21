using System.Configuration;
using System.Web;
using System.Web.Mvc;
using eMotive.CMS.Controllers;
using eMotive.CMS.IoC.Funq;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Service;
using Funq;
using ServiceStack.Mvc;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;


namespace eMotive.CMS
{
    public class AppHost : AppHostBase
    {
        public AppHost() : base("Intramed Web Services", typeof(Api.Services.Events.NewEvent).Assembly) { }

        public override void Configure(Container container)
        {
            var repositoryConnectionString = ConfigurationManager.ConnectionStrings["Repositories"].ConnectionString ?? string.Empty;
            var repositoryMSSQLConnectionString = ConfigurationManager.ConnectionStrings["MSSQLRepositories"].ConnectionString ?? string.Empty;
            
            FunqBindings.Configure(container);
            //Rather than reference web and owin stuff in bindings dll, we bind IAuditService here as the references are availible in the web project
            //container.Register<IAuditService>(c => new AuditService(c.Resolve<IConfigurationService>(), HttpContext.Current.GetOwinContext().Authentication, repositoryConnectionString)).ReusedWithin(ReuseScope.Request);
            container.Register<IAuditService>(c => new AuditService(c.Resolve<IConfigurationService>(), HttpContext.Current.GetOwinContext().Authentication, repositoryMSSQLConnectionString)).ReusedWithin(ReuseScope.Request);

            ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
            ServiceStackController.CatchAllController = reqCtx => container.TryResolve<TestController>();
            JsConfig.DateHandler = JsonDateHandler.ISO8601; 
        }
    }
}