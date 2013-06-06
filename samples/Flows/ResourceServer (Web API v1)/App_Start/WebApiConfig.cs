using System.Web.Http;
using Thinktecture.IdentityModel;
using Thinktecture.IdentityModel.Tokens.Http;
using System.Linq;

namespace Thinktecture.Samples
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.EnableSystemDiagnosticsTracing();
            config.MessageHandlers.Add(new AuthenticationHandler(CreateAuthenticationConfiguration()));
        }

        private static AuthenticationConfiguration CreateAuthenticationConfiguration()
        {
            var authentication = new AuthenticationConfiguration
            {
                //ClaimsAuthenticationManager = new ClaimsTransformer(),
                RequireSsl = false,
            };

            authentication.AddMsftJsonWebToken(
                issuer: Constants.AuthzSrv.IssuerName,
                audience: Constants.Audience,
                signingKey: Constants.AuthzSrv.SigningKey);

            return authentication;
        }
    }
}
