using Microsoft.IdentityModel.Tokens.JWT;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security.Tokens;
using Thinktecture.IdentityModel.Constants;
using Thinktecture.IdentityModel.Tokens.Http;

namespace Thinktecture.Samples
{
    public static class AuthenticationConfigurationExtensionsForMsftJwt
    {
        public static void AddMsftJsonWebToken(this AuthenticationConfiguration configuration, string issuer, string audience, string signingKey)
        {
            var validationParameters = new TokenValidationParameters()
            {
                AllowedAudience = audience,
                SigningToken = new BinarySecretSecurityToken(Convert.FromBase64String(signingKey)),
                ValidIssuer = issuer,
                ValidateExpiration = true
            };

            var handler = new JWTSecurityTokenHandlerWrapper(validationParameters);

            configuration.AddMapping(new AuthenticationOptionMapping
            {
                TokenHandler = new SecurityTokenHandlerCollection { handler },
                Options = AuthenticationOptions.ForAuthorizationHeader(JwtConstants.Bearer),
                Scheme = AuthenticationScheme.SchemeOnly(JwtConstants.Bearer)
            });
        }

        public static void AddMsftJsonWebToken(this AuthenticationConfiguration configuration, string issuer, string audience, X509Certificate2 signingCertificate)
        {
            var validationParameters = new TokenValidationParameters()
            {
                AllowedAudience = audience,
                SigningToken = new X509SecurityToken(signingCertificate),
                ValidIssuer = issuer,
                ValidateExpiration = true
            };

            var handler = new JWTSecurityTokenHandlerWrapper(validationParameters);

            configuration.AddMapping(new AuthenticationOptionMapping
            {
                TokenHandler = new SecurityTokenHandlerCollection { handler },
                Options = AuthenticationOptions.ForAuthorizationHeader(JwtConstants.Bearer),
                Scheme = AuthenticationScheme.SchemeOnly(JwtConstants.Bearer)
            });
        }
    }

    class JWTSecurityTokenHandlerWrapper : JWTSecurityTokenHandler
    {
        TokenValidationParameters validationParams;
        public JWTSecurityTokenHandlerWrapper(TokenValidationParameters validationParams)
        {
            this.validationParams = validationParams;
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<System.Security.Claims.ClaimsIdentity> ValidateToken(SecurityToken token)
        {
            var jwt = token as JWTSecurityToken;
            var list = new List<ClaimsIdentity>(this.ValidateToken(jwt, validationParams).Identities);
            return list.AsReadOnly();
        }
    }
}