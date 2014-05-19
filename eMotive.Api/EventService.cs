using System;
using System.Collections.Generic;
using System.Linq;
using eMotive.CMS.Extensions;
using eMotive.CMS.Models.Objects.Event;
using eMotive.CMS.Models.Objects.Json;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;
using eMotive.CMS.Services.Objects.EmailService;
using eMotive.CMS.Services.Objects.EventManagerService;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace eMotive.Api.Services.Events
{
    [Route("/Events/Audit", "GET")]
    public class GetAudit
    {
        public int Id { get; set; }

    }

    [Route("/Events/Audit/RollBack", "POST")]
    public class RollBack
    {
        public AuditRecord record { get; set; }
    }

    [Route("/Events/New", "GET")]
    public class NewEvent
    {
    }

    [Route("/Events/AssignationTypes", "GET")]
    public class EventAssignationTypes
    {
    }

    [Route("/Events/Application", "GET")]
    public class GetApplicationEvents
    {
        public int ApplicationID { get; set; }
    }

    [Route("/Events")]
    public class GetEvent
    {
        public int[] Ids { get; set; }
    }
    // IEnumerable<int> FetchEventItems(Type Object, int idEvent); 
    [Route("/Events/ForType")]
    public class GetIdsForEventType
    {
        public string Type { get; set; }
        public int EventId { get; set; }
    }

    [Route("/Events", "DELETE")]
    public class DeleteEvent
    {
        public int Id { get; set; }
    }


    [Route("/Events", "POST")]
    [Route("/Events", "PUT")]
    public class SaveEvent
    {
        public EventDescription Event { get; set; }
    }

    [Route("/Events/Application", "PUT")]
    public class SaveApplicationEvents
    {
        public EventDescription[] Events { get; set; }
        public int ApplicationID { get; set; }
    }

    [Route("/Events/AssignToEvent", "POST")]
    [Route("/Events/AssignToEvent", "PUT")]
    public class AssignToEvent
    {
        public EventObject eventObject { get; set; }
    }


    //http://stackoverflow.com/questions/15231537/recommended-servicestack-api-structure/15235822#15235822
    public class EventService : Service
    {
        private readonly IEventManagerService _eventManager;
        private readonly IMessageBusService _messageBusService;
        private readonly IAuditService _auditService;

        public EventService(IEventManagerService eventManager, IMessageBusService messageBusService, IAuditService auditService)
        {
            _eventManager = eventManager;
            _messageBusService = messageBusService;
            _auditService = auditService;
        }

        public object Get(NewEvent request)
        {
            return new ServiceResult<EventDescription>
            {
                Success = true,
                Result = _eventManager.New(),
                Errors = new string[] { }
            };

        }

        public object Get(EventAssignationTypes request)
        {
            return new ServiceResult<string[]>
            {
                Success = true,
                Result = new [] {"Email","Notification"},
                Errors = new string[] { }
            };

        }

        public object Get(GetIdsForEventType request)
        {
            //TODO: IS THERE A BETTER WAY OF DOING THIS?? SEEmS A BIT HARDCODED. PERHAPS HAVE AN EVENT TYPE TABLE CONTINING THE QUALIFIED TYPE NAMES?? 
            Type type = null;

            switch (request.Type)
            {
                case "Email":
                    type = typeof (Email);
                    break;
            }

            var result = _eventManager.FetchEventItems(type, request.EventId);

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            var success = issues.IsEmpty();
           
            return new ServiceResult<IEnumerable<int>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };
        }


        public object Get(GetApplicationEvents request)
        {
            var result = request.ApplicationID <= 0
                ? _eventManager.Fetch()
                : _eventManager.FetchForApplication(request.ApplicationID);

            var success = !result.IsEmpty();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<EventDescription>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };
        }

        public object Get(GetEvent request)
        {
            var result = _eventManager.Fetch(request.Ids);

            var success = !result.IsEmpty();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<EventDescription>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };
        }


        public object Post(AssignToEvent request)
        {

            switch (request.eventObject.Type)
            {
                case "Email":
                    request.eventObject.Type = typeof(Email).ToString();
                    break;
            }

            var success = _eventManager.AssignToEvent(request.eventObject);
          //  var success = _eventManager.RollBack(request.record);

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<bool>
            {
                Success = success,
                Result = success,
                Errors = issues
            };

        }

        public object Post(RollBack request)
        {
            var success = _eventManager.RollBack(request.record);

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
            var result = _auditService.FetchLog<EventDescription>(request.Id);

            var success = result.HasContent();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<AuditRecord>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };
        }

        public object Post(SaveEvent request)
        {
            int id;
            var success = true;

            success = _eventManager.Create(request.Event, out id);

            request.Event.ID = id;

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<EventDescription>
            {
                Success = success,
                Result = request.Event,
                Errors = issues
            };
        }

        public object Put(SaveApplicationEvents request)
        {
            var success = true;

            success = _eventManager.BulkProcessApplicationEvents(request.Events, request.ApplicationID);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<IEnumerable<EventDescription>>
            {
                Success = success,
                Result = success ? _eventManager.FetchForApplication(request.ApplicationID) : request.Events,
                Errors = issues
            };
        }

        public object Put(SaveEvent request)
        {
            var success = _eventManager.Update(request.Event);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<EventDescription>
            {
                Success = success,
                Result = request.Event,
                Errors = issues
            };
        }

        public object Delete(DeleteEvent request)
        {
            var success = _eventManager.Delete(request.Id);

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
}