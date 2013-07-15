/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class TokenRequestValidator
    {
        IStoredGrantManager _handleManager;

        public TokenRequestValidator()
        {

        }

        public TokenRequestValidator(IStoredGrantManager handleManager)
        {
            _handleManager = handleManager;
        }

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

            // check supported grant types
            if (request.Grant_Type == OAuthConstants.GrantTypes.AuthorizationCode ||
                request.Grant_Type == OAuthConstants.GrantTypes.ClientCredentials ||
                request.Grant_Type == OAuthConstants.GrantTypes.RefreshToken ||
                request.Grant_Type == OAuthConstants.GrantTypes.Password)
            {
                validatedRequest.GrantType = request.Grant_Type;
                Tracing.Information("Grant type: " + validatedRequest.GrantType);
            }
            else
            {
                throw new TokenRequestValidationException(
                    "Invalid grant_type: " + request.Grant_Type,
                    OAuthConstants.Errors.UnsupportedGrantType);
            }

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
                case OAuthConstants.GrantTypes.Password:
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
                    OAuthConstants.Errors.UnauthorizedClient);
            }

            ValidateScopes(validatedRequest, request);
        }

        private void ValidateRefreshTokenGrant(ValidatedRequest validatedRequest, TokenRequest request)
        {
            if (_handleManager == null)
            {
                throw new ArgumentNullException("HandleManager");
            }

            if (!validatedRequest.Client.AllowRefreshToken)
            {
                throw new TokenRequestValidationException(
                    "Refresh tokens not allowed for client",
                    OAuthConstants.Errors.UnauthorizedClient);
            }

            // check for refresh token
            if (string.IsNullOrWhiteSpace(request.Refresh_Token))
            {
                throw new TokenRequestValidationException(
                    "Missing refresh token",
                    OAuthConstants.Errors.InvalidGrant);
            }

            validatedRequest.RefreshToken = request.Refresh_Token;
            Tracing.Information("Refresh token: " + validatedRequest.RefreshToken);

            // check for refresh token in datastore
            var handle = _handleManager.Get(validatedRequest.RefreshToken);
            if (handle == null)
            {
                throw new TokenRequestValidationException(
                    "Refresh token not found: " + validatedRequest.RefreshToken,
                    OAuthConstants.Errors.InvalidGrant);
            }

            validatedRequest.TokenHandle = handle;
            Tracing.Information("Token handle found: " + handle.GrantId);

            // make sure the refresh token has an expiration time
            if (validatedRequest.TokenHandle.Expiration == null)
            {
                throw new TokenRequestValidationException(
                    "No expiration time set for refresh token. That's not allowed.",
                    OAuthConstants.Errors.InvalidGrant);
            }

            // make sure refresh token has not expired
            if (DateTime.UtcNow > validatedRequest.TokenHandle.Expiration)
            {
                throw new TokenRequestValidationException(
                    "Refresh token expired.",
                    OAuthConstants.Errors.InvalidGrant);
            }

            // check the client binding
            if (handle.Client.ClientId != validatedRequest.Client.ClientId)
            {
                throw new TokenRequestValidationException(
                    string.Format("Client {0} is trying to refresh token from {1}.", validatedRequest.Client.ClientId, handle.Client.ClientId),
                    OAuthConstants.Errors.InvalidGrant);
            }
        }

        private void ValidatePasswordGrant(ValidatedRequest validatedRequest, TokenRequest request)
        {
            if (validatedRequest.Client.Flow != OAuthFlow.ResourceOwner)
            {
                throw new TokenRequestValidationException(
                    "Resource owner password flow not allowed for client",
                    OAuthConstants.Errors.UnauthorizedClient);
            }

            ValidateScopes(validatedRequest, request);

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

        private static void ValidateScopes(ValidatedRequest validatedRequest, TokenRequest request)
        {
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
        }

        private void ValidateCodeGrant(ValidatedRequest validatedRequest, TokenRequest request)
        {
            if (_handleManager == null)
            {
                throw new ArgumentNullException("HandleManager");
            }

            if (validatedRequest.Client.Flow != OAuthFlow.Code)
            {
                throw new TokenRequestValidationException(
                    "Code flow not allowed for client",
                    OAuthConstants.Errors.UnauthorizedClient);
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

            // check for authorization code in datastore
            var handle = _handleManager.Get(validatedRequest.AuthorizationCode);
            if (handle == null)
            {
                throw new TokenRequestValidationException(
                    "Authorization code not found: " + validatedRequest.AuthorizationCode,
                    OAuthConstants.Errors.InvalidGrant);
            }

            validatedRequest.TokenHandle = handle;
            Tracing.Information("Token handle found: " + handle.GrantId);

            // check the client binding
            if (handle.Client.ClientId != validatedRequest.Client.ClientId)
            {
                throw new TokenRequestValidationException(
                    string.Format("Client {0} is trying to request token using an authorization code from {1}.", validatedRequest.Client.ClientId, handle.Client.ClientId),
                    OAuthConstants.Errors.InvalidGrant);
            }

            // redirect URI is required
            if (string.IsNullOrWhiteSpace(request.Redirect_Uri))
            {
                throw new TokenRequestValidationException(
                    string.Format("Redirect URI is missing"),
                    OAuthConstants.Errors.InvalidRequest);
            }

            // check if redirect URI from authorize and token request match
            if (!handle.RedirectUri.Equals(request.Redirect_Uri))
            {
                throw new TokenRequestValidationException(
                    string.Format("Redirect URI in token request ({0}), does not match redirect URI from authorize request ({1})", validatedRequest.RedirectUri, handle.RedirectUri),
                    OAuthConstants.Errors.InvalidRequest);
            }
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
