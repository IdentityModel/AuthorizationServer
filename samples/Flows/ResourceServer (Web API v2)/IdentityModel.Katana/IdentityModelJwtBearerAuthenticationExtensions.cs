using Microsoft.Owin.Security.Jwt;
using System;

namespace Owin
{
    public static class IdentityModelJwtBearerAuthenticationExtensions
    {
        public static IAppBuilder UseJsonWebToken(this IAppBuilder app, string issuer, string audience, string signingKey)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            var op = new JwtBearerAuthenticationOptions
            {
                AllowedAudiences = new[] { audience },
                IssuerSecurityTokenProviders = new[] 
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(
                            issuer,
                            signingKey)
                    }
            };

            app.UseJwtBearerAuthentication(op);
            
            return app;
        }
    }
}