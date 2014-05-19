using System;

namespace eMotive.CMS.Services.Objects.Audit
{
    public class AuditRecord
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Username { get; set; } //TODO: change this to user obj? How to do that from usermanager if usermanager is ref this though??
        public ActionType Action { get; set; }
        public string Object { get; set; }
        public string Details { get; set; }
    }
}
