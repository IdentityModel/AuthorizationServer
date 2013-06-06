using System.Web.Mvc;
using Thinktecture.IdentityModel.Clients;
using Thinktecture.Samples;

namespace OAuth2CodeFlow.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var url = OAuth2Client.CreateCodeFlowUrl(
                Constants.AuthzSrv.OAuth2AuthorizeEndpoint,
                Constants.Clients.CodeClient,
                "read search",
                Constants.Clients.CodeClientRedirectUrl);
            
            ViewBag.AuthorizeUrl = url;

            return View();
        }
    }
}
