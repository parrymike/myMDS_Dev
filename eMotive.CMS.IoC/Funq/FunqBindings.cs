using System.Configuration;
using AutoMapper;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Managers.Objects;
using eMotive.CMS.Managers.Objects.Managers;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Repositories.Objects.Repository;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Search.Objects;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Service;
using Funq;

namespace eMotive.CMS.IoC.Funq
{
    public static class FunqBindings
    {
        public static void Configure(Container container)
        {
            var loggingConnectionString = ConfigurationManager.ConnectionStrings["Logging"].ConnectionString ?? string.Empty;
            var repositoryConnectionString = ConfigurationManager.ConnectionStrings["Repositories"].ConnectionString ?? string.Empty;
            var luceneIndex = ConfigurationManager.AppSettings["LuceneIndex"] ?? string.Empty;
            container.Register(c => Mapper.Engine);


            #region repositories
            container.Register<IPageRepository>(c => new PageRepository(repositoryConnectionString)).ReusedWithin(ReuseScope.Request);
            container.Register<IUserRepository>(c => new UserRepository(repositoryConnectionString)).ReusedWithin(ReuseScope.Request);
            container.Register<ICourseRepository>(c => new CourseRepository(repositoryConnectionString)).ReusedWithin(ReuseScope.Request);
            container.Register<IApplicationRepository>(c => new ApplicationRepository(repositoryConnectionString)).ReusedWithin(ReuseScope.Request);
            #endregion


            #region services
            container.Register<IConfigurationService>(c => new ConfigurationService()).ReusedWithin(ReuseScope.Request);
            container.Register<IMessageBusService>(c => new MessageBusService()).ReusedWithin(ReuseScope.Request);
            container.Register<IDocumentManagerService>(c => new DocumentManagerService(c.Resolve<IServiceRepository>())).ReusedWithin(ReuseScope.Request);

            container.Register<IEventManagerService>(c => new EventManagerService(repositoryConnectionString) { AuditService = c.Resolve<IAuditService>() }).ReusedWithin(ReuseScope.Request);

            container.Register<IEmailService>(c => new EmailService(repositoryConnectionString)
            {
                AuditService = c.Resolve<IAuditService>(),
                MessageBusService = c.Resolve<IMessageBusService>(),
                SearchManager = null//c.Resolve<ISearchManager>(),
            }).ReusedWithin(ReuseScope.Request);
            #endregion

            #region managers
            //container.Register<ISearchManager>(c => new SearchManager(luceneIndex)).ReusedWithin(ReuseScope.Container);

            /*  container.Register<IRoleManager>(c => new RoleManager(c.Resolve<IRoleRepository>())
              {
                  EventManagerService = c.Resolve<IEventManagerService>(),
                  Mapper = c.Resolve<IMappingEngine>(),
                  SearchManager = null,//c.Resolve<ISearchManager>(),
                  MessageBusService = c.Resolve<IMessageBusService>(),
                  AuditService = c.Resolve<IAuditService>()
              }).ReusedWithin(ReuseScope.Request);*/

            container.Register<IPageManager>(c => new PageManager(c.Resolve<IPageRepository>())
            {
                EventManagerService = c.Resolve<IEventManagerService>(),
                Mapper = c.Resolve<IMappingEngine>(),
                SearchManager = null,//c.Resolve<ISearchManager>(),
                MessageBusService = c.Resolve<IMessageBusService>(),
                AuditService = c.Resolve<IAuditService>()
            }).ReusedWithin(ReuseScope.Request);


            container.Register<ICourseManager>(c => new CourseManager(c.Resolve<ICourseRepository>())
                        {
                            EventManagerService = c.Resolve<IEventManagerService>(),
                            Mapper = c.Resolve<IMappingEngine>(),
                            SearchManager = null,//c.Resolve<ISearchManager>(),
                            MessageBusService = c.Resolve<IMessageBusService>(),
                            AuditService = c.Resolve<IAuditService>()
                        }).ReusedWithin(ReuseScope.Request);

            container.Register<IApplicationManager>(c => new ApplicationManager(c.Resolve<IApplicationRepository>())
            {
                EventManagerService = c.Resolve<IEventManagerService>(),
                Mapper = c.Resolve<IMappingEngine>(),
                SearchManager = null,//c.Resolve<ISearchManager>(),
                MessageBusService = c.Resolve<IMessageBusService>(),
                AuditService = c.Resolve<IAuditService>()
            }).ReusedWithin(ReuseScope.Request);

            container.Register<IUserManager>(c => new UserManager(c.Resolve<IUserRepository>())
            {
                EventManagerService = c.Resolve<IEventManagerService>(),
                Mapper = c.Resolve<IMappingEngine>(),
                //SearchManager = null,//c.Resolve<ISearchManager>(),
                MessageBusService = c.Resolve<IMessageBusService>(),
                //AuditService = c.Resolve<IAuditService>()
            }).ReusedWithin(ReuseScope.Request);

            container.Register<INavigationManager>(c => new NavigationManager());
            // Bind<ISearchManager>().To<SearchManager>().InSingletonScope().WithConstructorArgument("_indexLocation", ConfigurationManager.AppSettings["LuceneIndex"]);

            #endregion
        }
    }
}
