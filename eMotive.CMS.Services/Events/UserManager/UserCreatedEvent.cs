using System.Collections.Generic;
using System.Text;
using eMotive.CMS.Extensions;
using eMotive.CMS.Models.Objects.Courses;
using eMotive.CMS.Models.Objects.Users;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.EmailService;
using ServiceStack.WebHost.Endpoints;

namespace eMotive.CMS.Services.Events.UserManager
{
    public class UserCreatedEvent : IEvent
    {
        private readonly User _user;

        public UserCreatedEvent(User user)
        {
            _user = user;
        }

        public void Fire()
        {
            var eventManagerService = AppHostBase.Instance.TryResolve<IEventManagerService>();
            var emailService = AppHostBase.Instance.TryResolve<IEmailService>();


            var emailIds = eventManagerService.FetchEventItems(typeof(Email), "UserCreatedEvent");

            if (!emailIds.IsEmpty())
            {
                var emails = emailService.Fetch(emailIds);

                var replacements = new Dictionary<string, string>(5)
                    {
                       /* {"#forename#", "UnknownForename"},
                        {"#surname#", "UnknownSurname"},
                        {"#username#", "UnknownUsername"},
                        {"#coursename#", _course.Name},
                        {"#courseabbreviation#", _course.Abbreviation}*/
                    };

                var sbSubject = new StringBuilder();
                var sbBody = new StringBuilder();

                foreach (var email in emails ?? new Email[] { })
                {
                    sbSubject.Append(email.Subject);
                    sbBody.Append(email.Body);

                    if (replacements.HasContent())
                    {
                        foreach (var replacment in replacements)
                        {
                            sbBody.Replace(replacment.Key, replacment.Value);
                            sbSubject.Replace(replacment.Key, replacment.Value);
                        }

                        email.Subject = sbSubject.ToString();
                        email.Body = sbBody.ToString();
                    }

                    sbSubject.Length = 0;
                    sbBody.Length = 0;
                }

                emailService.Send(emails, null);
            }
            //send an email etc
            // throw new Exception(courseName);
        }
    }
}
