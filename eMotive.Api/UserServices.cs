using System.Collections.Generic;
using System.Linq;
using eMotive.CMS.Extensions;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Courses;
using eMotive.CMS.Models.Objects.Json;
using eMotive.CMS.Models.Objects.Roles;
using eMotive.CMS.Models.Objects.Users;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace eMotive.Api.Services.Users
{

    [Route("/Users/New", "GET")]
    public class NewUser
    {
    }

    [Route("/Users/Audit", "GET")]
    public class GetAudit
    {
        public int Id { get; set; }

    }

    [Route("/Users/Audit/RollBack", "POST")]
    public class RollBack
    {
        public AuditRecord record { get; set; }
    }


    [Route("/Users")]
    [Route("/Users/{Ids}")]
    public class GetUsers
    {
        public int[] Ids { get; set; }
    }


    [Route("/Users", "DELETE")]
    public class DeleteUser
    {
        public int Id { get; set; }
    }

    [Route("/Users", "POST")]
    [Route("/Users", "PUT")]
   
    public class SaveUser
    {
        public User User { get; set; }
    }

    [Route("/Users/Search", "POST")]
    public class DoSearch
    {
        public RoleSearch RoleSearch { get; set; }
    }

    //http://stackoverflow.com/questions/15231537/recommended-servicestack-api-structure/15235822#15235822
    public class UserService : Service
    {
        private readonly IUserManager _userManager;
        private readonly IMessageBusService _messageBusService;
        private readonly IAuditService _auditService;

        public UserService(IUserManager userManager, IMessageBusService messageBusService, IAuditService auditService)
        {
            _userManager = userManager;
            _messageBusService = messageBusService;
            _auditService = auditService;
        }

        public object Get(NewUser request)
        {
            return new ServiceResult<User>
            {
                Success = true,
                Result = _userManager.New(),
                Errors = new string[] { }
            };

        }
        
        public object Post(RollBack request)
        {
            var success = _userManager.RollBack(request.record);

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
            var result = _auditService.FetchLog<User>(request.Id);

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

        public object Get(GetUsers request)
        {
            var result = request.Ids.IsEmpty()
                ? _userManager.FetchAll()
                : _userManager.Fetch(request.Ids);

            var success = !result.IsEmpty();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<User>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };

        }

        public object Post(SaveUser request)
        {
            int id;
            var success = _userManager.Create(request.User, out id);

            if (success)
                request.User.ID = id;

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<User>
            {
                Success = success,
                Result = request.User,
                Errors = issues
            };
        }

        public object Put(SaveUser request)
        {
            var success = _userManager.Update(request.User);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<User>
            {
                Success = success,
                Result = request.User,
                Errors = issues
            };
        }

        public object Delete(DeleteUser request)
        {
            var success = _userManager.Delete(request.Id);

            var issues = _messageBusService.Fetch().Select(m => m.Details);

            return new ServiceResult<bool>
            {
                Success = success,
                Result = success,
                Errors = issues
            };
        }
    }
}