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
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer
{
    public class TokenService
    {
        GlobalConfiguration globalConfiguration;

        public TokenService(GlobalConfiguration globalConfiguration)
        {
            this.globalConfiguration = globalConfiguration;
        }

        public virtual TokenResponse CreateTokenResponse(TokenHandle handle, ITokenHandleManager handleManager)
        {
            //handleManager.Delete(handle.HandleId);

            var resourceOwner = Principal.Create(
                "OAuth2",
                handle.ResourceOwner.ToClaims().ToArray());

            var validatedRequest = new ValidatedRequest
            {
                Client = handle.Client,
                Application = handle.Application,
                Scopes = handle.Scopes
            };

            var response = CreateTokenResponse(validatedRequest, resourceOwner);

            if (handle.CreateRefreshToken)
            {
                var refreshTokenHandle = TokenHandle.CreateRefreshTokenHandle(
                    handle.Client,
                    handle.Application,
                    resourceOwner.Claims,
                    handle.Scopes,
                    handle.RefreshTokenExpiration);

                handleManager.Add(refreshTokenHandle);
                response.RefreshToken = refreshTokenHandle.HandleId;
            }

            return response;
        }

        public virtual TokenResponse CreateTokenResponse(ValidatedRequest request, ClaimsPrincipal resourceOwner)
        {
            try
            {
                var claims = CreateClaims(request, resourceOwner);
                var token = CreateToken(request, claims);

                return new TokenResponse
                {
                    AccessToken = WriteToken(token),
                    ExpiresIn = request.Application.TokenLifetime * 60,
                    TokenType = "Bearer"
                };
            }
            catch (Exception ex)
            {
                Tracing.Error(ex.ToString());
                throw;
            }
        }

        protected virtual string WriteToken(JWTSecurityToken token)
        {
            return new JWTSecurityTokenHandler().WriteToken(token);
        }

        protected virtual JWTSecurityToken CreateToken(ValidatedRequest request, IEnumerable<Claim> claims)
        {
            var token = new JWTSecurityToken(
                issuer: globalConfiguration.Issuer,
                audience: request.Application.Audience,
                claims: claims,
                signingCredentials: request.Application.SigningCredentials,
                validFrom: DateTime.UtcNow,
                validTo: DateTime.UtcNow.AddMinutes(request.Application.TokenLifetime));

            return token;
        }

        protected virtual IEnumerable<Claim> CreateClaims(ValidatedRequest request, ClaimsPrincipal resourceOwner)
        {
            var claims = new List<Claim>();

            claims.AddRange(CreateRequestClaims(request));
            claims.AddRange(CreateResourceOwnerClaims(resourceOwner));

            return claims;
        }

        protected virtual IEnumerable<Claim> CreateResourceOwnerClaims(ClaimsPrincipal resourceOwner)
        {
            return resourceOwner.FilterInternalClaims();
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
