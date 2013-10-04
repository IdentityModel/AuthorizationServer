using System;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Client;
using Thinktecture.Samples;

namespace OAuth2CodeFlow.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var client = new OAuth2Client(new Uri(Constants.AS.OAuth2AuthorizeEndpoint));

            var url = client.CreateCodeFlowUrl(
                Constants.Clients.CodeClient,
                "read search",
                Constants.Clients.CodeClientRedirectUrl);
            
            ViewBag.AuthorizeUrl = url;

            return View();
        }
    }
}
