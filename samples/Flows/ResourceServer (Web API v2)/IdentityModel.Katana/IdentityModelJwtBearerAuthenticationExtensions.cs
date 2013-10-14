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

            //var options = new OAuthBearerAuthenticationOptions
            //{
            //    Realm = audience,
            //    AccessTokenFormat = new JwtFormat(
            //        audience, 
            //        new SymmetricKeyIssuerSecurityTokenProvider(
            //            issuer,
            //            signingKey)),
            //};

            app.UseJwtBearerAuthentication(op);
            //app.UseOAuthBearerAuthentication(options);
            return app;
        }
    }
}