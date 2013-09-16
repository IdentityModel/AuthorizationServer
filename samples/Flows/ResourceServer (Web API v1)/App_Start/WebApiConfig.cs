using System.Web.Http;
using Thinktecture.IdentityModel.Tokens;
using Thinktecture.IdentityModel.Tokens.Http;

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

            var corsConfig = new Thinktecture.IdentityModel.Http.Cors.WebApi.WebApiCorsConfiguration();
            corsConfig.ForResources("Identity").ForOrigins("https://localhost:44300").AllowAll();
            var handler = new Thinktecture.IdentityModel.Http.Cors.WebApi.CorsMessageHandler(corsConfig, config);
            config.MessageHandlers.Add(handler);

            config.EnableSystemDiagnosticsTracing();
            config.MessageHandlers.Add(
                new AuthenticationHandler(CreateAuthenticationConfiguration()));
        }

        private static AuthenticationConfiguration CreateAuthenticationConfiguration()
        {
            var authentication = new AuthenticationConfiguration
            {
                RequireSsl = false,
            };

            authentication.AddJsonWebToken(
                issuer: Constants.AS.IssuerName,
                audience: Constants.Audience,
                signingKey: Constants.AS.SigningKey,
                claimMappings: ClaimMappings.None);

            return authentication;
        }
    }
}
