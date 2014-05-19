using System;
using System.Security.Claims;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using ServiceStack;

//http://stackoverflow.com/questions/13478515/what-is-the-cleanest-way-to-leverage-forms-authentication-from-servicestack

//http://mono.servicestack.net/ServiceStack.Hello/
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;

namespace eMotive.CMS.Controllers
{
    public class AccountController : Controller
    {
        IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        //
       // GET: /Account/
        [HttpGet]
        public ActionResult Login()
        {
         //   var authService = AppHostBase.Instance.TryResolve<AuthService>();
          //  authService.RequestContext = CreateRequestContext();
            var authService = AppHostBase.Instance.TryResolve<AuthProvider>();

            throw new NotImplementedException();
           // return View();
        }
        
        /*

        [HttpPost]
        public ActionResult Login(string  username)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username), }, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);

                                // if you want roles, just add as many as you want here (for loop maybe?)
                    identity.AddClaim(new Claim(ClaimTypes.Role, "guest"));
                    // tell OWIN the identity provider, optional
                    // identity.AddClaim(new Claim(IdentityProvider, "Simplest Auth"));

                    Authentication.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, identity);

            return View(new Login());
        }*/
	}
}