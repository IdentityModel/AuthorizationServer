using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Clients;
using Thinktecture.Samples;

namespace OAuth2CodeFlow.Controllers
{
    public class CallbackController : Controller
    {
        static Uri _baseAddress = new Uri(Constants.WebHostv1BaseAddress);

        public ActionResult Index()
        {
            ViewBag.Code = Request.QueryString["code"] ?? "none";
            ViewBag.Error = Request.QueryString["error"] ?? "none";


            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Postback()
        {
            var client = new OAuth2Client(
                new Uri(Constants.AS.OAuth2TokenEndpoint),
                Constants.Clients.CodeClient,
                Constants.Clients.CodeClientSecret);

            var code = Request.QueryString["code"];

            var response = client.RequestAccessTokenCode(
                code,
                new Uri(Constants.Clients.CodeClientRedirectUrl));

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

            var claims = response.Content.ReadAsAsync<IEnumerable<ViewClaim>>().Result;

            return View("Claims", claims);
        }

        [HttpPost]
        public ActionResult RenewToken(string refreshToken)
        {
            var client = new OAuth2Client(
                new Uri(Constants.AS.OAuth2TokenEndpoint),
                Constants.Clients.CodeClient,
                Constants.Clients.CodeClientSecret);
            var response = client.RequestAccessTokenRefreshToken(refreshToken);
            return View("Postback", response);
        }
    }



    public class ViewClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

}
