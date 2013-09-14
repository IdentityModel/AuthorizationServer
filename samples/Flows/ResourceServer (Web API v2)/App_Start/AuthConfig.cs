using Microsoft.Owin.Security.Jwt;
using Owin;

namespace Thinktecture.Samples
{
    public static class AuthConfig
    {
        public static void Configure(IAppBuilder app)
        {
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AllowedAudiences = new[] { Constants.Audience },
                IssuerSecurityTokenProviders = new[] { new SymmetricKeyIssuerSecurityTokenProvider(
                        Constants.AS.IssuerName,
                        Constants.AS.SigningKey) }
            });
        }
    }
}