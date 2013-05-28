using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Interfaces;

namespace WebHost.Controllers
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

    }
}
