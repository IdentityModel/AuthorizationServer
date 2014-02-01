using Microsoft.Owin;
using Owin;
using System.IdentityModel.Tokens;
using Thinktecture.IdentityModel;
using Thinktecture.IdentityModel.Tokens;
using Thinktecture.Samples.Security;

[assembly: OwinStartup(typeof(Thinktecture.Samples.Startup))]

namespace Thinktecture.Samples
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // authorization manager
            ClaimsAuthorization.CustomAuthorizationManager = new AuthorizationManager();

            // no mapping of incoming claims to Microsoft types
            JwtSecurityTokenHandler.InboundClaimTypeMap = ClaimMappings.None;

            // validate JWT tokens from AuthorizationServer
            app.UseJsonWebToken(
                issuer: Constants.AS.IssuerName,
                audience: Constants.Audience,
                signingKey: Constants.AS.SigningKey);

            // claims transformation
            app.UseClaimsTransformation(new ClaimsTransformer().Transform);

            app.UseWebApi(WebApiConfig.Configure());
        }
    }
}