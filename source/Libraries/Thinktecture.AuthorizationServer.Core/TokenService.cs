using Microsoft.IdentityModel.Tokens.JWT;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.AuthorizationServer.Core.Models;

namespace Thinktecture.AuthorizationServer.Core
{
    public class TokenService
    {
        public TokenResponse CreateToken(ValidatedRequest request, ClaimsPrincipal resourceOwner)
        {
            var subject = CreateSubject(request, resourceOwner);
            var descriptor = CreateDescriptor(request, subject);
            var token = CreateToken(descriptor);

            return new TokenResponse
            {
                AccessToken = token,
                ExpiresIn = request.Application.TokenLifetime,
                TokenType = "Bearer",
                RefreshToken = "todo"
            };
        }

        private string CreateToken(SecurityTokenDescriptor descriptor)
        {
            var handler = new JWTSecurityTokenHandler();
            
            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }

        protected virtual SecurityTokenDescriptor CreateDescriptor(ValidatedRequest request, ClaimsIdentity subject)
        {
            var descriptor = new SecurityTokenDescriptor
            {
                AppliesToAddress = request.Application.Audience,
                Lifetime = new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(request.Application.TokenLifetime)),
                TokenIssuerName = request.Application.IssuerName,
                Subject = subject,
                SigningCredentials = request.Application.GetSigningCredentials()
            };

            return descriptor;
        }

        protected virtual ClaimsIdentity CreateSubject(ValidatedRequest request, ClaimsPrincipal resourceOwner)
        {
            var claims = new List<Claim>();

            claims.AddRange(CreateRequestClaims(request));
            claims.AddRange(CreateResourceOwnerClaims(resourceOwner));

            var subject = new ClaimsIdentity(claims, "tt.authz");
            return subject;
        }

        protected virtual List<Claim> CreateResourceOwnerClaims(ClaimsPrincipal resourceOwner)
        {
            return resourceOwner.Claims.ToList();
        }

        protected virtual List<Claim> CreateRequestClaims(ValidatedRequest request)
        {
            var claims = new List<Claim>
            {
                new Claim("client_id", request.Client.ClientId)
            };

            request.Scopes.ForEach(s => claims.Add(new Claim("scope", s.Name)));

            return claims;
        }
    }
}
