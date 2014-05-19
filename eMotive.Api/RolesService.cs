using System.Collections.Generic;
using System.Linq;
using eMotive.CMS.Extensions;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Json;
using eMotive.CMS.Models.Objects.Roles;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace eMotive.Api.Services.Roles
{

    [Route("/Roles/New", "GET")]
    public class NewRole
    {
    }

    [Route("/Roles/Audit", "GET")]
    public class GetAudit
    {
        public int Id { get; set; }

    }

    [Route("/Roles/Audit/RollBack","POST")]
    public class RollBack
    {
        public AuditRecord record { get; set; }
    }


    [Route("/Roles")]
    [Route("/Roles/{Ids}")]
    public class GetRoles
    {
        public int[] Ids { get; set; }
    }

    [Route("/Roles", "POST")]
    [Route("/Roles", "PUT")]
    [Route("/Roles", "DELETE")]
    public class SaveRole
    {
        public Role Role { get; set; }
    }

    [Route("/Roles/Search", "POST")]
    public class DoSearch
    {
        public RoleSearch RoleSearch { get; set; }
    }

    //http://stackoverflow.com/questions/15231537/recommended-servicestack-api-structure/15235822#15235822
    public class RolesService : Service
    {
        private readonly IRoleManager _roleManager;
        private readonly IMessageBusService _messageBusService;
        private readonly IAuditService _auditService;

        public RolesService(IRoleManager roleManager, IMessageBusService messageBusService, IAuditService auditService)
        {
            _roleManager = roleManager;
            _messageBusService = messageBusService;
            _auditService = auditService;
        }

        public object Get(NewRole request)
        {
            return new ServiceResult<Role>
            {
                Success = true,
                Result = _roleManager.New(),
                Errors = new string[] { }
            };

        }

        public object Post(RollBack request)
        {
            var success = _roleManager.RollBack(request.record);

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
            var result = _auditService.FetchLog<Role>(request.Id);

            var success = result.HasContent();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<AuditRecord>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };

        }

        public object Post(DoSearch request)
        {

            var searchItem = _roleManager.DoSearch(request.RoleSearch);

            if (searchItem.Items.HasContent())
            {
                request.RoleSearch.NumberOfResults = searchItem.NumberOfResults;
                request.RoleSearch.Roles = _roleManager.FetchRecordsFromSearch(searchItem);
            }

            var success = request.RoleSearch.Roles.HasContent();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<RoleSearch>
            {
                Success = success,
                Result = request.RoleSearch,
                Errors = issues
            };

        }

        public object Get(GetRoles request)
        {
            var result = request.Ids.IsEmpty()
                ? _roleManager.FetchAll()
                : _roleManager.Fetch(request.Ids);

            var success = result.HasContent();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<Role>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };

        }

        public object Post(SaveRole request)
        {
            int id;
            var success = _roleManager.Create(request.Role, out id);

            if (success)
                request.Role.ID = id;

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Role>
            {
                Success = success,
                Result = request.Role,
                Errors = issues
            };
        }

        public object Put(SaveRole request)
        {
            var success = _roleManager.Update(request.Role);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Role>
            {
                Success = success,
                Result = request.Role,
                Errors = issues
            };
        }

        public object Delete(SaveRole request)
        {
            var success = _roleManager.Delete(request.Role);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            if (success)
                request.Role = null;

            return new ServiceResult<Role>
            {
                Success = success,
                Result = request.Role,
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