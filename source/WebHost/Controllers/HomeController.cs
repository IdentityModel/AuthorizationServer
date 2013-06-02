using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.WebHost.Controllers
{
    public class HomeController : Controller
    {
        IAuthorizationServerConfiguration config;
        public HomeController(IAuthorizationServerConfiguration config)
        {
            this.config = config;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Banner()
        {
            ViewData["ServerName"] = config.GlobalConfiguration.AuthorizationServerName;
            return PartialView("Banner");
        }

        public ActionResult Footer()
        {
            return PartialView("Footer");
        }

    }
}
