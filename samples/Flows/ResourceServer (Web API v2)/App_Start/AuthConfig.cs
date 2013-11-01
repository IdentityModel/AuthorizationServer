using Owin;
using System.Collections.Generic;
using System.IdentityModel.Tokens;

namespace Thinktecture.Samples
{
    public static class AuthConfig
    {
        public static void Configure(IAppBuilder app)
        {
            // no mapping of incoming claims to Microsoft types
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            // validate JWT tokens from AuthorizationServer
            app.UseJsonWebToken(
                issuer:     Constants.AS.IssuerName,
                audience:   Constants.Audience,
                signingKey: Constants.AS.SigningKey);

            // claims transformation
            app.UseClaimsTransformation(new ClaimsTransformer());
        }
    }
}