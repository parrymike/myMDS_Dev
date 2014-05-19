using System;
using System.Collections.Generic;
using eMotive.CMS.Models.Objects.Event;
using eMotive.CMS.Services.Objects.EventManagerService;

namespace eMotive.CMS.Services.Interfaces
{
    public interface IEventManagerService : IAuditable
    {
        void QueueEvent(IEvent eventToFire);
        void FireEvents();

        EventDescription Fetch(int Id);
    //    EventDescription Fetch(string Name);

        IEnumerable<EventDescription> Fetch();
        IEnumerable<EventDescription> Fetch(IEnumerable<int> Ids);

        IEnumerable<EventDescription> FetchForApplication(int Id);

        EventDescription New();

        bool BulkProcessApplicationEvents(IEnumerable<EventDescription> newAppEvents, int applicationId);

        bool Create(EventDescription eventDescription, out int id);
        bool Update(EventDescription eventDescription);
        bool Delete(int id);
        bool Put(EventDescription eventDescription);

       // bool AssignToEvent<T>(Func<T, int> idField, T Object, int IdEvent) where T : class; 
        bool AssignToEvent(EventObject eventObject); 
        IEnumerable<int> FetchEventItems(Type Object, int idEvent);
        IEnumerable<int> FetchEventItems(Type Object, string eventName);
        IEnumerable<int> FetchEventItems(string eventName); 
    }
}
