using Microsoft.Owin.Security.Jwt;
using Owin;
using System.IdentityModel.Tokens;
using Thinktecture.IdentityModel.Tokens;

namespace Thinktecture.Samples
{
    public static class AuthConfig
    {
        public static void Configure(IAppBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap = ClaimMappings.None;

            app.UseJwtBearerToken(
                issuer:     Constants.AS.IssuerName,
                audience:   Constants.Audience,
                signingKey: Constants.AS.SigningKey);

            #region Katana
            //app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            //{
            //    AllowedAudiences = new[] { Constants.Audience },
            //    IssuerSecurityTokenProviders = new[] { new SymmetricKeyIssuerSecurityTokenProvider(
            //            Constants.AS.IssuerName,
            //            Constants.AS.SigningKey) }
            //});
            #endregion
        }
    }
}