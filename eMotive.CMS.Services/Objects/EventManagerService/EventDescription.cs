using System.Collections.Generic;

namespace eMotive.CMS.Services.Objects.EventManagerService
{
    public class EventDescription
    {
        public int ID { get; set; }
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public string NiceName { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool System { get; set; }

        public IEnumerable<EventTag> Tags { get; set; } 
    }
}
