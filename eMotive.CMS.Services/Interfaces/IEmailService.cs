using System.Collections.Generic;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Services.Objects.EmailService;

namespace eMotive.CMS.Services.Interfaces
{
    //TODO: Either use IEventAttachable OR just use the Fetch(IEnumerable IDS bit?)
    public interface IEmailService : IAuditable, ISearchable<Email>
    {
        bool Send(Email email/*, IDictionary<string, string> replacements*/, BinaryAttachment[] binaryAttachments);
        bool Send(IEnumerable<Email> emails/*, IDictionary<string, string> replacements*/, BinaryAttachment[] binaryAttachments);


        Email New();
        bool Create(Email email, out int id);
        bool Update(Email email);
        bool Delete(int id);
        bool Put(Email email);

        IEnumerable<Email> Fetch();
        IEnumerable<Email> Fetch(IEnumerable<int> ids);
    }
}
