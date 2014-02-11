using Microsoft.Owin;
using Owin;
using System.IdentityModel.Tokens;
using Thinktecture.IdentityModel.Tokens;

[assembly: OwinStartup(typeof(Thinktecture.Samples.Startup))]

namespace Thinktecture.Samples
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // no mapping of incoming claims to Microsoft types
            JwtSecurityTokenHandler.InboundClaimTypeMap = ClaimMappings.None;
            
            // validate JWT tokens from AuthorizationServer
            app.UseJsonWebToken(
                issuer: Constants.AS.IssuerName,
                audience: Constants.Audience,
                signingKey: Constants.AS.SigningKey);

            // claims transformation
            app.UseClaimsTransformation(new ClaimsTransformer().Transform);

            app.UseNancy();
        }
    }
}