using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Application;
using eMotive.CMS.Models.Objects.Search;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Search.Objects;
using eMotive.CMS.Services.Events.ApplicationManager;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects;
using eMotive.CMS.Services.Objects.Audit;
using Rep = eMotive.CMS.Repositories.Objects;
using Mod = eMotive.CMS.Models;
using Map = eMotive.CMS.Managers.AutoMapperConfiguration.Maps;

namespace eMotive.CMS.Managers.Objects.Managers
{
    public class ApplicationManager : IApplicationManager
    {
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationManager(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
            AutoMapperConfiguration.Configure(Map.Application);
        }

        public IMappingEngine Mapper { get; set; }
        public IEventManagerService EventManagerService { get; set; }
        public ISearchManager SearchManager { get; set; }
        public IMessageBusService MessageBusService { get; set; }
        public IAuditService AuditService { get; set; }


        public Application New()
        {
            var app = _applicationRepository.New();

            return Mapper.Map<Rep.Application.Application, Application>(app);
        }

        public bool Create(Application application, out int id)
        {
            id = -1;

            var checkApplication = _applicationRepository.Fetch(application.Name);

            if (checkApplication != null)
            {

                //if (checkApplication.Archived)
                 //{
                //     MessageBusService.AddIssue("");//ResourceManager.ApplicationManager_Create_ApplicationArchived()
                  //   return false;
                // }

                if (String.Equals(application.Name, checkApplication.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBusService.AddIssue(string.Format("An application with the name '{0}' already exists.", application.Name));//ResourceManager.CourseManager_Create_CourseNameExists()
                    return false;
                }

            }

            var uniqueRoleNames = !application.Roles.GroupBy(n => n.Name).Any(m => m.Count() > 1);

            if (!uniqueRoleNames)
            {
                MessageBusService.AddIssue("Role names should be unique!");
                return false;
            }

            var repApplication = Mapper.Map<Application, Rep.Application.Application>(application);
            if (_applicationRepository.Create(repApplication, out id))
            {
                var newApp = Fetch(id);
                AuditService.ObjectAuditLog(ActionType.Create, n => n.ID, newApp);

                EventManagerService.QueueEvent(new ApplicationCreatedEvent(newApp));

                return true;
            }

            //   MessageBusService.AddError(ResourceManager.CourseManager_Create_Error());
            return false;
        }

        public bool Update(Application application)
        {
            var checkApplication = _applicationRepository.Fetch(application.Name);

            if (checkApplication != null)
            {
                if (String.Equals(application.Name, checkApplication.Name, StringComparison.InvariantCultureIgnoreCase) && application.ID != checkApplication.ID)
                {
                    MessageBusService.AddIssue(string.Format("An application with the name '{0}' already exists.", application.Name));
                    return false;
                }
            }

            var uniqueRoleNames = !application.Roles.GroupBy(n => n.Name).Any(m => m.Count() > 1);

            if (!uniqueRoleNames)
            {
                MessageBusService.AddIssue("Role names should be unique!");
                return false;
            }


            var repApp = Mapper.Map<Application, Rep.Application.Application>(application);
            if (_applicationRepository.Update(repApp))
            {
                var updated = Fetch(application.ID);
                AuditService.ObjectAuditLog(ActionType.Update, n => n.ID, updated);
                EventManagerService.QueueEvent(new ApplicationUpdatedEvent(updated));

                return true;
            }

            // MessageBusService.AddError(ResourceManager.CourseManager_Update_Error());
            return false;
        }

        public bool Delete(int id)
        {
            //var repApp = Mapper.Map<Course, Rep.Courses.Course>(course);
            var application = Fetch(id);
            if (_applicationRepository.Delete(Mapper.Map<Application, Rep.Application.Application>(application)))
            {
                AuditService.ObjectAuditLog(ActionType.Delete, n => n.ID, application);

                EventManagerService.QueueEvent(new ApplicationDeletedEvent(application));
                return true;
            }

            // MessageBusService.AddError(ResourceManager.ApplicationManager_Delete_Error());
            return false;
        }

        public IEnumerable<Application> Fetch()
        {
            var applications = _applicationRepository.Fetch();

            //if (courses.IsEmpty())
            //   MessageBusService.AddIssue(ResourceManager.CourseManager_Fetch_FetchError());

            return Mapper.Map<IEnumerable<Rep.Application.Application>, IEnumerable<Application>>(applications);
        }

        public IEnumerable<Application> Fetch(IEnumerable<int> ids)
        {
            var applications = _applicationRepository.Fetch(ids);

            //if (courses.IsEmpty())
            //   MessageBusService.AddIssue(ResourceManager.CourseManager_Fetch_FetchError());

            return Mapper.Map<IEnumerable<Rep.Application.Application>, IEnumerable<Application>>(applications);
        }

        public Application Fetch(int id)
        {
            var app = _applicationRepository.Fetch(id);

            return Mapper.Map<Rep.Application.Application, Application>(app);
        }

        public Application Fetch(string name)
        {
            var app = _applicationRepository.Fetch(name);

            return Mapper.Map<Rep.Application.Application, Application>(app);
        }


        public bool RollBack(AuditRecord record)
        {
            var rollBackApplication = record.Object.FromJson<Application>();
            var repApp = Mapper.Map<Application, Rep.Application.Application>(rollBackApplication);
            var success = _applicationRepository.Put(repApp);

            if (success)
            {
                AuditService.ObjectAuditLog(ActionType.RollBack, n => n.ID, rollBackApplication, record);
                EventManagerService.QueueEvent(new ApplicationRolledBackEvent(rollBackApplication));
            }

            return success;
        }


        public void ReindexSearchRecords()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Application> FetchRecordsFromSearch(SearchResult searchResult)
        {
            throw new NotImplementedException();
        }

        public SearchResult DoSearch(BasicSearch search)
        {
            throw new NotImplementedException();
        }
    }
}
