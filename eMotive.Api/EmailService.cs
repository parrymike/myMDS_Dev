using System.Collections.Generic;
using System.Linq;
using eMotive.CMS.Extensions;
using eMotive.CMS.Models.Objects.Courses;
using eMotive.CMS.Models.Objects.Json;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;
using eMotive.CMS.Services.Objects.EmailService;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace eMotive.Api.Services.Emails
{
    //todo: Need fetchEmailsFromEvent here?? YESSSSSSSSSSSSSSSSS
    [Route("/Emails/New", "GET")]
    public class NewEmail
    {
    }

    [Route("/Emails/Audit", "GET")]
    public class GetAudit
    {
        public int Id { get; set; }

    }

    [Route("/Emails/Audit/RollBack", "POST")]
    public class RollBack
    {
        public AuditRecord record { get; set; }
    }


    [Route("/Emails")]
    [Route("/Emails/{Ids}")]
    public class GetEmails
    {
        public int[] Ids { get; set; }
    }


    [Route("/Emails", "DELETE")]
    public class DeleteEmail
    {
        public int Id { get; set; }
    }

    [Route("/Emails", "POST")]
    [Route("/Emails", "PUT")]
    public class SaveEmail
    {
        public Email email { get; set; }
    }
    /*
    [Route("/Emails/Event", "POST")]
    [Route("/Emails/Event", "PUT")]
    public class SaveEmailEvent : EventObject<Email>
    {
        
    }*/

    [Route("/Emails/Search", "POST")]
    public class DoSearch
    {
       // public RoleSearch RoleSearch { get; set; }
    }

    //http://stackoverflow.com/questions/15231537/recommended-servicestack-api-structure/15235822#15235822
    public class EmailService : Service
    {
        private readonly IEmailService _emailService;
        private readonly IMessageBusService _messageBusService;
        private readonly IAuditService _auditService;
      //  private readonly IEventManagerService _eventManagerService;

        public EmailService(IEmailService emailService, IMessageBusService messageBusService, IAuditService auditService/*, IEventManagerService eventManagerService*/)
        {
            _emailService = emailService;
            _messageBusService = messageBusService;
            _auditService = auditService;
         //   _eventManagerService = eventManagerService;
        }

        public object Get(NewEmail request)
        {
            return new ServiceResult<Email>
            {
                Success = true,
                Result = _emailService.New(),
                Errors = new string[] { }
            };
        
        }

        public object Post(RollBack request)
        {
            var success = _emailService.RollBack(request.record);

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
            var result = _auditService.FetchLog<Email>(request.Id);

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

        public object Get(GetEmails request)
        {
            var result = request.Ids.IsEmpty()
                ? _emailService.Fetch()
                : _emailService.Fetch(request.Ids);

            var success = !result.IsEmpty();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<Email>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };

        }
        /*
        public object Post(SaveEmailEvent request)
        {
         /*   int id;
            var success = _emailService.Create(request.EventDescription, out id);

            if (success)
                request.EventDescription.ID = id;

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;
            */
        /*    return new ServiceResult<Email>
            {
                Success = false,//success,
                Result = null,//request.EventDescription,
                Errors = null//issues
            };
        }

        public object Put(SaveEmailEvent request)
        {
          /*  var success = _emailService.Update(request.EventDescription);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Email>
            {
                Success = success,
                Result = request.EventDescription,
                Errors = issues
            };*/
          /*  return new ServiceResult<Email>
            {
                Success = false,//success,
                Result = null,//request.EventDescription,
                Errors = null//issues
            };
        }*/

        public object Post(SaveEmail request)
        {
            int id;
            var success = _emailService.Create(request.email, out id);

            if (success)
                request.email.ID = id;

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Email>
            {
                Success = success,
                Result = request.email,
                Errors = issues
            };
        }

        public object Put(SaveEmail request)
        {
            var success = _emailService.Update(request.email);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Email>
            {
                Success = success,
                Result = request.email,
                Errors = issues
            };
        }

        public object Delete(DeleteEmail request)
        {
            var success = _emailService.Delete(request.Id);

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