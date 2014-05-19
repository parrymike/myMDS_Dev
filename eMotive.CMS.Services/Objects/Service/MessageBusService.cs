using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.MessageBusService;


[assembly: InternalsVisibleTo("eMotive.CMS.Tests")]
namespace eMotive.CMS.Services.Objects.Service
{
    public class MessageBusService : IMessageBusService
    {
        internal readonly ICollection<Message> Messages;

        public MessageBusService()
        {
            Messages = new Collection<Message>();
        }

        public void AddIssue(string issue)
        {
            Messages.Add(new Message { Details = issue, IsError = false });

        }

        public void AddError(string error)
        {
            Messages.Add(new Message { Details = error, IsError = true });
        }

        public IEnumerable<Message> Fetch()
        {
            return Messages;
        }
    }
}
