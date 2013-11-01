using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Client;
using Thinktecture.Samples;

namespace OAuth2CodeFlow.Controllers
{
    public class CallbackController : Controller
    {
        //static Uri _baseAddress = new Uri(Constants.WebHostv1BaseAddress);
        static Uri _baseAddress = new Uri(Constants.WebHostv2BaseAddress);

        public ActionResult Index()
        {
            ViewBag.Code = Request.QueryString["code"] ?? "none";
            ViewBag.Error = Request.QueryString["error"] ?? "none";


            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<ActionResult> Postback()
        {
            var client = new OAuth2Client(
                new Uri(Constants.AS.OAuth2TokenEndpoint),
                Constants.Clients.CodeClient,
                Constants.Clients.CodeClientSecret);

            var code = Request.QueryString["code"];

            var response = await client.RequestAuthorizationCodeAsync(
                code,
                Constants.Clients.CodeClientRedirectUrl);

            return View("Postback", response);
        }

        [HttpPost]
        [ActionName("CallService")]
        public ActionResult CallService(string token)
        {
            var client = new HttpClient
            {
                BaseAddress = _baseAddress
            };

            client.SetBearerToken(token);

            var response = client.GetAsync("identity").Result;
            response.EnsureSuccessStatusCode();

            var claims = response.Content.ReadAsAsync<IEnumerable<Models.ViewClaim>>().Result;

            return View("Claims", claims);
        }

        [HttpPost]
        public async Task<ActionResult> RenewToken(string refreshToken)
        {
            var client = new OAuth2Client(
                new Uri(Constants.AS.OAuth2TokenEndpoint),
                Constants.Clients.CodeClient,
                Constants.Clients.CodeClientSecret);

            var response = await client.RequestRefreshTokenAsync(refreshToken);
            return View("Postback", response);
        }
    }
}
