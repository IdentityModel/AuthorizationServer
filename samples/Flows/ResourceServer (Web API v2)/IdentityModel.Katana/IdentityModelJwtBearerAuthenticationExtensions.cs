using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using System;

namespace Owin
{
    public static class IdentityModelJwtBearerAuthenticationExtensions
    {
        public static IAppBuilder UseJwtBearerToken(this IAppBuilder app, string issuer, string audience, string signingKey)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            OAuthBearerAuthenticationOptions options = new OAuthBearerAuthenticationOptions
            {
                Realm = audience,
                AccessTokenFormat = new JwtFormat(
                    audience, 
                    new SymmetricKeyIssuerSecurityTokenProvider(
                        issuer,
                        signingKey)),
            };

            app.UseOAuthBearerAuthentication(options);
            return app;
        }
    }
}