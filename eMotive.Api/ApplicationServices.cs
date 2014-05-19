using System.Collections.Generic;
using System.Linq;
using eMotive.CMS.Extensions;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Application;
using eMotive.CMS.Models.Objects.Json;
using eMotive.CMS.Models.Objects.Roles;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace eMotive.Api.Services.Applications
{

    [Route("/Applications/New", "GET")]
    public class NewApplication
    {
    }

    [Route("/Applications/Audit", "GET")]
    public class GetAudit
    {
        public int Id { get; set; }

    }

    [Route("/Applications/Audit/RollBack", "POST")]
    public class RollBack
    {
        public AuditRecord record { get; set; }
    }


    [Route("/Applications")]
    [Route("/Applications/{Ids}")]
    public class GetApplications
    {
        public int[] Ids { get; set; }
    }


    [Route("/Applications", "DELETE")]
    public class DeleteApplication
    {
        public int Id { get; set; }
    }

    [Route("/Applications", "POST")]
    [Route("/Applications", "PUT")]

    public class SaveApplication
    {
        public Application Application { get; set; }
    }

    [Route("/Applications/Search", "POST")]
    public class DoSearch
    {
        public RoleSearch RoleSearch { get; set; }
    }

    //http://stackoverflow.com/questions/15231537/recommended-servicestack-api-structure/15235822#15235822
    public class ApplicationServices : Service
    {
        private readonly IApplicationManager _applicationManager;
        private readonly IMessageBusService _messageBusService;
        private readonly IAuditService _auditService;

        public ApplicationServices(IApplicationManager applicationManager, IMessageBusService messageBusService, IAuditService auditService)
        {
            _applicationManager = applicationManager;
            _messageBusService = messageBusService;
            _auditService = auditService;
        }

        public object Get(NewApplication request)
        {
            return new ServiceResult<Application>
            {
                Success = true,
                Result = _applicationManager.New(),
                Errors = new string[] { }
            };

        }

        public object Post(RollBack request)
        {
            var success = _applicationManager.RollBack(request.record);

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<bool>
            {
                Success = success,
                Result = success,
                Errors = issues
            };

        }

        public object Get(GetAudit request)
        {
            var result = _auditService.FetchLog<Application>(request.Id);

            var success = result.HasContent();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<AuditRecord>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };

        }
        /*
        public object Post(DoSearch request)
        {

            var searchItem = _emailService.DoSearch(request.RoleSearch);

            if (searchItem.Items.HasContent())
            {
                request.RoleSearch.NumberOfResults = searchItem.NumberOfResults;
                request.RoleSearch.Roles = _emailService.FetchRecordsFromSearch(searchItem);
            }

            var success = request.RoleSearch.Roles.HasContent();

            var issues = MessageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<RoleSearch>
            {
                Success = success,
                Result = request.RoleSearch,
                Errors = issues
            };

        }*/

        public object Get(GetApplications request)
        {
            var result = request.Ids.IsEmpty()
                ? _applicationManager.Fetch()
                : _applicationManager.Fetch(request.Ids);

            var success = !result.IsEmpty();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<Application>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };

        }

        public object Post(SaveApplication request)
        {
            int id;
            var success = _applicationManager.Create(request.Application, out id);

            if (success)
                request.Application.ID = id;

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Application>
            {
                Success = success,
                Result = request.Application,
                Errors = issues
            };
        }

        public object Put(SaveApplication request)
        {
            var success = _applicationManager.Update(request.Application);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Application>
            {
                Success = success,
                Result = request.Application,
                Errors = issues
            };
        }

        public object Delete(DeleteApplication request)
        {
            var success = _applicationManager.Delete(request.Id);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

        //    if (success)
             //   request.Course = null;

            return new ServiceResult<bool>
            {
                Success = success,
                Result = success,
                Errors = issues
            };
        }
    }

   /* public class RolesResponse<T>
    {
        public bool Success;
        public IEnumerable<T> Result { get; set; }
        public ResponseStatus ResponseStatus { get; set; } //Where Exceptions get auto-serialized
    }*/
}