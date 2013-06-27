using System.Collections.Generic;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.WebHost.Areas.InitialConfiguration.Models;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.InitialConfiguration.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        IAuthorizationServerAdministration authorizationServerAdministration;
        public HomeController(IAuthorizationServerAdministration authorizationServerAdministration)
        {
            this.authorizationServerAdministration = authorizationServerAdministration;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (authorizationServerAdministration.GlobalConfiguration != null)
            {
                filterContext.Result = new RedirectResult("~");
            }
        }

        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(InitialConfigurationModel model)
        {
            if (ModelState.IsValid)
            {
                var global = new GlobalConfiguration()
                {
                    AuthorizationServerName = model.Name,
                    Issuer = model.Issuer,
                    Administrators = new List<AuthorizationServerAdministrator>
                    {
                        new AuthorizationServerAdministrator{NameID = model.Admin}
                    }
                };
                authorizationServerAdministration.GlobalConfiguration = global;
                authorizationServerAdministration.SaveChanges();

                if (model.Test == "test")
                {
                    TestData.Populate();
                }

                return View("Success");
            }

            return View("Index");
        }
    }
}
