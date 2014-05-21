using System.Collections.Generic;
using System.Text;
using eMotive.CMS.Extensions;
using eMotive.CMS.Models.Objects.Courses;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.EmailService;
using ServiceStack.WebHost.Endpoints;

namespace eMotive.CMS.Services.Events.CourseManager
{
    public class CourseUpdatedEvent : IEvent
    {
        private readonly Course _course;

        public CourseUpdatedEvent(Course course)
        {
            _course = course;
        }

        public void Fire()
        {
            var eventManagerService = AppHostBase.Instance.TryResolve<IEventManagerService>();
            var emailService = AppHostBase.Instance.TryResolve<IEmailService>();


            var emailIds = eventManagerService.FetchEventItems(typeof(Email), "CourseUpdatedEvent");

            if (!emailIds.IsEmpty())
            {
                var emails = emailService.Fetch(emailIds);

                var replacements = new Dictionary<string, string>(4)
                    {//TODO: have old and new course in here? i.e for coursename change??
                        {"#forename#", "UnknownForename"},
                        {"#surname#", "UnknownSurname"},
                        {"#username#", "UnknownUsername"},
                        {"#coursename#", _course.Name},
                        {"#courseabbreviation#", _course.Abbreviation}
                    };

                var sbSubject = new StringBuilder();
                var sbBody = new StringBuilder();

                foreach (var email in emails ?? new Email[] {})
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
