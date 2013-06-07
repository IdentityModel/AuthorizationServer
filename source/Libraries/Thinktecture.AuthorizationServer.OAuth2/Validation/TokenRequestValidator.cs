/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class TokenRequestValidator
    {
        public ValidatedRequest Validate(Application application, TokenRequest request, ClaimsPrincipal clientPrincipal)
        {
            var validatedRequest = new ValidatedRequest();

            // validate request model binding
            if (request == null)
            {
                throw new TokenRequestValidationException(
                    "Invalid request parameters.",
                    OAuthConstants.Errors.InvalidRequest);
            }

            validatedRequest.Application = application;
            Tracing.InformationFormat("OAuth2 application: {0} ({1})",
                validatedRequest.Application.Name,
                validatedRequest.Application.Namespace);

            // grant type is required
            if (string.IsNullOrWhiteSpace(request.Grant_Type))
            {
                throw new TokenRequestValidationException(
                    "Missing grant_type",
                    OAuthConstants.Errors.UnsupportedGrantType);
            }

            validatedRequest.GrantType = request.Grant_Type;
            Tracing.Information("Grant type: " + validatedRequest.GrantType);

            // validate client credentials
            var client = ValidateClient(clientPrincipal, validatedRequest.Application);
            if (client == null)
            {
                throw new TokenRequestValidationException(
                    "Invalid client: " + ClaimsPrincipal.Current.Identity.Name,
                    OAuthConstants.Errors.InvalidClient);
            }

            validatedRequest.Client = client;
            Tracing.InformationFormat("Client: {0} ({1})",
                validatedRequest.Client.Name,
                validatedRequest.Client.ClientId);

            switch (request.Grant_Type)
            {
                case OAuthConstants.GrantTypes.AuthorizationCode:
                     ValidateCodeGrant(validatedRequest, request);
                     break;
                case OAuthConstants.GrantTypes.Password :
                    ValidatePasswordGrant(validatedRequest, request);
                    break;
                case OAuthConstants.GrantTypes.RefreshToken:
                    ValidateRefreshTokenGrant(validatedRequest, request);
                    break;
                case OAuthConstants.GrantTypes.ClientCredentials:
                    ValidateClientCredentialsGrant(validatedRequest, request);
                    break;
                default:
                    throw new TokenRequestValidationException(
                        "Invalid grant_type: " + request.Grant_Type,
                        OAuthConstants.Errors.UnsupportedGrantType);
            }

            Tracing.Information("Token request validation successful.");
            return validatedRequest;
        }

        private void ValidateClientCredentialsGrant(ValidatedRequest validatedRequest, TokenRequest request)
        {
            if (validatedRequest.Client.Flow != OAuthFlow.Client)
            {
                throw new TokenRequestValidationException(
                    "Client flow not allowed for client",
                    OAuthConstants.Errors.UnsupportedGrantType);
            }
        }

        private void ValidateRefreshTokenGrant(ValidatedRequest validatedRequest, TokenRequest request)
        {
            if (!validatedRequest.Client.AllowRefreshToken)
            {
                throw new TokenRequestValidationException(
                    "Refresh tokens not allowed for client",
                    OAuthConstants.Errors.UnsupportedGrantType);
            }

            // ...and a refresh token request need a refresh token
            if (validatedRequest.GrantType.Equals(OAuthConstants.GrantTypes.RefreshToken))
            {
                if (string.IsNullOrWhiteSpace(request.Refresh_Token))
                {
                    throw new TokenRequestValidationException(
                        "Missing refresh token",
                        OAuthConstants.Errors.InvalidGrant);
                }

                validatedRequest.RefreshToken = request.Refresh_Token;
                Tracing.Information("Refresh token: " + validatedRequest.RefreshToken);
            }
        }

        private void ValidatePasswordGrant(ValidatedRequest validatedRequest, TokenRequest request)
        {
            if (validatedRequest.Client.Flow != OAuthFlow.ResourceOwner)
            {
                throw new TokenRequestValidationException(
                    "Resource owner password flow not allowed for client",
                    OAuthConstants.Errors.UnsupportedGrantType);
            }

            // validate scope
            if (string.IsNullOrWhiteSpace(request.Scope))
            {
                throw new TokenRequestValidationException(
                    "Missing scope",
                    OAuthConstants.Errors.InvalidScope);
            }

            // make sure client is allowed to request all scope
            var requestedScopes = request.Scope.Split(' ').ToList();
            List<Scope> resultingScopes;

            if (validatedRequest.Application.Scopes.TryValidateScopes(validatedRequest.Client.ClientId, requestedScopes, out resultingScopes))
            {
                validatedRequest.Scopes = resultingScopes;
                Tracing.InformationFormat("Requested scopes: {0}", request.Scope);
            }
            else
            {
                throw new TokenRequestValidationException(
                    "Invalid scope",
                    OAuthConstants.Errors.InvalidScope);
            }

            // extract username and password
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new TokenRequestValidationException(
                    "Missing Username or password.",
                    OAuthConstants.Errors.InvalidGrant);
            }
            else
            {
                validatedRequest.UserName = request.UserName;
                validatedRequest.Password = request.Password;

                Tracing.Information("Resource owner: " + request.UserName);
            }
        }

        private void ValidateCodeGrant(ValidatedRequest validatedRequest, TokenRequest request)
        {
            if (validatedRequest.Client.Flow != OAuthFlow.Code)
            {
                throw new TokenRequestValidationException(
                    "Code flow not allowed for client",
                    OAuthConstants.Errors.UnsupportedGrantType);
            }
            
            // code needs to be present
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                throw new TokenRequestValidationException(
                    "Missing authorization code",
                    OAuthConstants.Errors.InvalidGrant);
            }

            validatedRequest.AuthorizationCode = request.Code;
            Tracing.Information("Authorization code: " + validatedRequest.AuthorizationCode);
        }

        private Client ValidateClient(ClaimsPrincipal clientPrincipal, Application application)
        {
            if (clientPrincipal == null || !clientPrincipal.Identity.IsAuthenticated)
            {
                Tracing.Error("Anonymous client.");
                return null;
            }

            var passwordClaim = clientPrincipal.FindFirst("password");
            if (passwordClaim == null)
            {
                Tracing.Error("No client secret provided.");
                return null;
            }

            return application.Clients.ValidateClient(
                clientPrincipal.Identity.Name,
                passwordClaim.Value);
        }
    }
}
