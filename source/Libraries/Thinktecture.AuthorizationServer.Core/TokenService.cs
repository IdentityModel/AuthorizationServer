/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Microsoft.IdentityModel.Tokens.JWT;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer
{
    public class TokenService
    {
        GlobalConfiguration globalConfiguration;

        public TokenService(GlobalConfiguration globalConfiguration)
        {
            this.globalConfiguration = globalConfiguration;
        }

        public TokenResponse CreateToken(ValidatedRequest request, ClaimsPrincipal resourceOwner)
        {
            try
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
            catch (Exception ex)
            {
                Tracing.Error(ex.ToString());
                throw;
            }
        }

        protected virtual string CreateToken(SecurityTokenDescriptor descriptor)
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
                TokenIssuerName = globalConfiguration.Issuer,
                Subject = subject,
                SigningCredentials = request.Application.SigningCredentials
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
