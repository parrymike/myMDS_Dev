using System.Collections.Generic;
using System.Net.Mail;
using eMotive.CMS.Services.Objects.DocumentManagerService;

namespace eMotive.CMS.Services.Objects.EmailService
{
    public class Email
    {
        public Email()
        {
    
        }

        public int ID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public MailPriority Priority { get; set; }
        public bool IsBodyHtml { get; set; }

        public bool Enabled { get; set; }
        
        public ICollection<Document> Attachments { get; set; }

        //todo: What about binary attachments?? i.e. system generated attachments 
    }
}
