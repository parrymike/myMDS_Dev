using System.Configuration;
using AutoMapper;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Managers.Objects.Managers;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Repositories.Objects.Repository;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Search.Objects;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Service;
using Ninject.Modules;
using Ninject.Web.Common;

namespace eMotive.CMS.IoC.Ninject
{
   /* public class eMotiveCMSModule: NinjectModule
    {
        public override void Load()
        {
            var loggingConnectionString = ConfigurationManager.ConnectionStrings["Logging"].ConnectionString ?? string.Empty;
            var repositoryConnectionString = ConfigurationManager.ConnectionStrings["Repositories"].ConnectionString ?? string.Empty;

            Bind<IMappingEngine>().ToMethod(ctx => Mapper.Engine);

            #region services
            Bind<IConfigurationService>().To<ConfigurationService>().InRequestScope();
            Bind<IMessageBusService>().To<MessageBusService>().InRequestScope();
            Bind<IAuditService>().To<AuditService>().InRequestScope();
            #endregion

            #region repositories
            Bind<IServiceRepository>().To<ServiceRepository>().InRequestScope().WithConstructorArgument("connectionString", repositoryConnectionString);
            Bind<IRoleRepository>().To<RoleRepository>().InRequestScope().WithConstructorArgument("connectionString", repositoryConnectionString);
            Bind<IUserRepository>().To<UserRepository>().InRequestScope().WithConstructorArgument("connectionString", repositoryConnectionString);
            #endregion
   
           #region managers
           Bind<IRoleManager>().To<RoleManager>().InRequestScope();
           Bind<ISearchManager>().To<SearchManager>().InSingletonScope().WithConstructorArgument("_indexLocation", ConfigurationManager.AppSettings["LuceneIndex"]);
           #endregion


        }
    }*/
}
