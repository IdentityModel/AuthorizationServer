using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using System;

namespace Owin
{
    public static class IdentityModelJwtBearerAuthenticationExtensions
    {
        public static IAppBuilder UseJsonWebToken(this IAppBuilder app, string issuer, string audience, string signingKey, string type = null, OAuthBearerAuthenticationProvider location = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            var options = new JwtBearerAuthenticationOptions
            {
                AllowedAudiences = new[] { audience },
                IssuerSecurityTokenProviders = new[] 
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(
                            issuer,
                            signingKey)
                    }
            };

            if (!string.IsNullOrEmpty(type))
            {
                options.AuthenticationType = type;
            }

            if (location != null)
            {
                options.Provider = location;
            }

            app.UseJwtBearerAuthentication(options);
            
            return app;
        }
    }
}