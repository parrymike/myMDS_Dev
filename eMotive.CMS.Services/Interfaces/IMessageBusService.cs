using System.Collections.Generic;
using eMotive.CMS.Services.Objects.MessageBusService;

namespace eMotive.CMS.Services.Interfaces
{
    /// <summary>
    /// A simple mssage bus. Allows two types of messages to be raised. An error will redirect to an Error page and display the errors.
    /// An issue is information that can be returned to a user as a warning e.g. Username already taken etc.
    /// </summary>
    public interface IMessageBusService
    {//TODO: should we have 'message' notification type? CW 12/03/2014 06:42

        void AddIssue(string issue);
        void AddError(string issue);

        IEnumerable<Message> Fetch();
    }
}
