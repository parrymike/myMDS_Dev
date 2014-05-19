using System.Collections.Generic;
using System.Linq;
using eMotive.CMS.Extensions;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Courses;
using eMotive.CMS.Models.Objects.Json;
using eMotive.CMS.Models.Objects.Roles;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace eMotive.Api.Services.Users
{

    [Route("/Courses/New", "GET")]
    public class NewCourse
    {
    }

    [Route("/Courses/Audit", "GET")]
    public class GetAudit
    {
        public int Id { get; set; }

    }

    [Route("/Courses/Audit/RollBack", "POST")]
    public class RollBack
    {
        public AuditRecord record { get; set; }
    }


    [Route("/Courses")]
    [Route("/Courses/{Ids}")]
    public class GetCourses
    {
        public int[] Ids { get; set; }
    }


    [Route("/Courses", "DELETE")]
    public class DeleteCourse
    {
        public int Id { get; set; }
    }

    [Route("/Courses", "POST")]
    [Route("/Courses", "PUT")]
   
    public class SaveCourse
    {
        public Course Course { get; set; }
    }

    [Route("/Courses/Search", "POST")]
    public class DoSearch
    {
        public RoleSearch RoleSearch { get; set; }
    }

    //http://stackoverflow.com/questions/15231537/recommended-servicestack-api-structure/15235822#15235822
    public class CourseService : Service
    {
        private readonly ICourseManager _courseManager;
        private readonly IMessageBusService _messageBusService;
        private readonly IAuditService _auditService;

        public CourseService(ICourseManager courseManager, IMessageBusService messageBusService, IAuditService auditService)
        {
            _courseManager = courseManager;
            _messageBusService = messageBusService;
            _auditService = auditService;
        }

        public object Get(NewCourse request)
        {
            return new ServiceResult<Course>
            {
                Success = true,
                Result = _courseManager.New(),
                Errors = new string[] { }
            };

        }

        public object Post(RollBack request)
        {
            var success = _courseManager.RollBack(request.record);

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
            var result = _auditService.FetchLog<Course>(request.Id);

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

        public object Get(GetCourses request)
        {
            var result = request.Ids.IsEmpty()
                ? _courseManager.Fetch()
                : _courseManager.Fetch(request.Ids);

            var success = !result.IsEmpty();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<Course>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };

        }

        public object Post(SaveCourse request)
        {
            int id;
            var success = _courseManager.Create(request.Course, out id);

            if (success)
                request.Course.ID = id;

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Course>
            {
                Success = success,
                Result = request.Course,
                Errors = issues
            };
        }

        public object Put(SaveCourse request)
        {
            var success = _courseManager.Update(request.Course);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Course>
            {
                Success = success,
                Result = request.Course,
                Errors = issues
            };
        }

        public object Delete(DeleteCourse request)
        {
            var success = _courseManager.Delete(request.Id);

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