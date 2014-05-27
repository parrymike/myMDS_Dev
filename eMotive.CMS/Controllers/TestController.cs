
using System.Web.Mvc;
//http://www.itworld.com/development/379646/aspnet-mvc-5-brings-attribute-based-routing
using eMotive.CMS.Managers.Interfaces;

using eMotive.CMS.Services.Interfaces;
using ServiceStack.Logging;
using ServiceStack.Mvc;

namespace eMotive.CMS.Controllers
{
    [RoutePrefix("test")]
   // [Route("{action}")]
    public class TestController : ServiceStackController
    {
      //  private readonly IRoleManager _roleManager;
        private readonly IEmailService _emailService;
        private readonly IMessageBusService _messageBusService;
      //  private readonly ILog _log;
        public TestController(/*IRoleManager roleManager,*/ IMessageBusService messageBusService, IEmailService emailService)//, ILog log)
        {
         //   _roleManager = roleManager;
            _messageBusService = messageBusService;
            _emailService = emailService;
           // _log = LogManager.GetLogger(GetType());
        }

        public ActionResult SignalRTest()
        {
            return View();
        }

        //[ActionName("CourseTest")]
        
        [Route("Course")]
        public ActionResult CourseTest()
        {
            return View();
        }

        [Route]
        [Route("~/")]
        [Route("Application")]
        public ActionResult ApplicationTest()
        {
            return View();
        }

        [Route("Emails")]
        public ActionResult EmailsTest()
        {
            return View();
        }

        [Route("Pages")]
        public ActionResult PagesTest()
        {
            return View();
        }

        [Route("Users")]
        public ActionResult UsersTest()
        {
            return View();
        }
	}
}