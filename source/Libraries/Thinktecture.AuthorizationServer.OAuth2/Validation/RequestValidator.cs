/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class RequestValidator
    {
        public ValidatedRequest ValidateAuthorizeRequest(Application application, AuthorizeRequest request)
        {
            // If the request fails due to a missing, invalid, or mismatching
            // redirection URI, or if the client identifier is missing or invalid,
            // the authorization server SHOULD inform the resource owner of the
            // error and MUST NOT automatically redirect the user-agent to the
            // invalid redirection URI.

            var validatedRequest = new ValidatedRequest();

            // validate request model binding
            if (request == null)
            {
                throw new AuthorizeRequestResourceOwnerException("Invalid request parameters.");
            }

            validatedRequest.Application = application;
            Tracing.InformationFormat("OAuth2 application: {0} ({1})",
                validatedRequest.Application.Name,
                validatedRequest.Application.Namespace);

            // make sure redirect uri is present
            if (string.IsNullOrWhiteSpace(request.redirect_uri))
            {
                throw new AuthorizeRequestResourceOwnerException("Missing redirect URI");
            }

            // check state
            if (!string.IsNullOrWhiteSpace(request.state))
            {
                validatedRequest.State = request.state;
                Tracing.Information("State: " + validatedRequest.State);
            }
            else
            {
                Tracing.Information("No state supplied.");
            }

            // validate client
            if (string.IsNullOrWhiteSpace(request.client_id))
            {
                throw new AuthorizeRequestResourceOwnerException("Missing client identifier");
            }

            var client = validatedRequest.Application.Clients.Get(request.client_id);
            if (client == null)
            {
                throw new AuthorizeRequestResourceOwnerException("Invalid client: " + request.client_id);
            }

            validatedRequest.Client = client;
            Tracing.InformationFormat("Client: {0} ({1})",
                validatedRequest.Client.Name,
                validatedRequest.Client.ClientId);

            // make sure redirect_uri is a valid uri, and in case of http is over ssl
            Uri redirectUri;
            if (Uri.TryCreate(request.redirect_uri, UriKind.Absolute, out redirectUri))
            {
                if (redirectUri.Scheme == Uri.UriSchemeHttp)
                {
                    throw new AuthorizeRequestClientException(
                        "Redirect URI not over SSL : " + request.redirect_uri,
                        new Uri(request.redirect_uri),
                        OAuthConstants.Errors.InvalidRequest,
                        string.Empty,
                        validatedRequest.State);
                }

                // make sure redirect uri is registered with client
                var validUri = validatedRequest.Client.RedirectUris.Get(request.redirect_uri);

                if (validUri == null)
                {
                    throw new AuthorizeRequestResourceOwnerException("Invalid redirect URI: " + request.redirect_uri);
                }

                validatedRequest.RedirectUri = validUri;
                Tracing.InformationFormat("Redirect URI: {0} ({1})",
                    validatedRequest.RedirectUri.Uri,
                    validatedRequest.RedirectUri.Description);
            }
            else
            {
                var message = "Invalid redirect URI: " + request.redirect_uri;
                Tracing.Error(message);

                throw new AuthorizeRequestResourceOwnerException("Invalid redirect URI: " + request.redirect_uri);
            }

            // validate response type
            if (String.IsNullOrWhiteSpace(request.response_type))
            {
                throw new AuthorizeRequestClientException(
                    "response_type is null or empty",
                    new Uri(validatedRequest.RedirectUri.Uri),
                    OAuthConstants.Errors.InvalidRequest,
                    string.Empty,
                    validatedRequest.State);
            }

            // check response type (only code and token are supported)
            if (!request.response_type.Equals(OAuthConstants.ResponseTypes.Token, StringComparison.Ordinal) &&
                !request.response_type.Equals(OAuthConstants.ResponseTypes.Code, StringComparison.Ordinal))
            {
                throw new AuthorizeRequestClientException(
                    "response_type is not token or code: " + request.response_type,
                    new Uri(validatedRequest.RedirectUri.Uri),
                    OAuthConstants.Errors.UnsupportedResponseType,
                    string.Empty,
                    validatedRequest.State);
            }

            // make sure response type allowed for this client
            if (validatedRequest.Client.Flow == OAuthFlow.Code &&
                request.response_type != OAuthConstants.ResponseTypes.Code)
            {
                throw new AuthorizeRequestClientException(
                   "response_type is not allowed: " + request.response_type,
                   new Uri(validatedRequest.RedirectUri.Uri),
                   OAuthConstants.Errors.UnsupportedResponseType,
                   request.response_type,
                   validatedRequest.State);
            }

            if (validatedRequest.Client.Flow == OAuthFlow.Implicit &&
                request.response_type != OAuthConstants.ResponseTypes.Token)
            {
                throw new AuthorizeRequestClientException(
                    "response_type is not allowed: " + request.response_type,
                    new Uri(validatedRequest.RedirectUri.Uri),
                    OAuthConstants.Errors.UnsupportedResponseType,
                    request.response_type,
                    validatedRequest.State);
            }

            validatedRequest.ResponseType = request.response_type;
            Tracing.Information("Response type: " + validatedRequest.ResponseType);

            // validate scopes
            if (string.IsNullOrEmpty(request.scope))
            {
                throw new AuthorizeRequestClientException(
                    "Missing scope.",
                    new Uri(validatedRequest.RedirectUri.Uri),
                    OAuthConstants.Errors.InvalidScope,
                    validatedRequest.ResponseType,
                    validatedRequest.State);
            }

            var requestedScopes = request.scope.Split(' ').ToList();
            List<Scope> resultingScopes;

            if (validatedRequest.Application.Scopes.TryValidateScopes(validatedRequest.Client.ClientId, requestedScopes, out resultingScopes))
            {
                validatedRequest.Scopes = resultingScopes;
                Tracing.InformationFormat("Requested scopes: {0}", request.scope);
            }
            else
            {
                throw new AuthorizeRequestClientException(
                    "Invalid scope.",
                    new Uri(validatedRequest.RedirectUri.Uri),
                    OAuthConstants.Errors.InvalidScope,
                    validatedRequest.ResponseType,
                    validatedRequest.State);
            }

            Tracing.Information("Authorize request validation successful.");
            return validatedRequest;
        }

        public ValidatedRequest ValidateTokenRequest(Application application, TokenRequest request)
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
            if (!request.Grant_Type.Equals(OAuthConstants.GrantTypes.AuthorizationCode) &&
                !request.Grant_Type.Equals(OAuthConstants.GrantTypes.Password) &&
                !request.Grant_Type.Equals(OAuthConstants.GrantTypes.RefreshToken) &&
                !request.Grant_Type.Equals(OAuthConstants.GrantTypes.ClientCredentials))
            {
                throw new TokenRequestValidationException(
                    "Invalid grant_type: " + request.Grant_Type,
                    OAuthConstants.Errors.UnsupportedGrantType);
            }

            validatedRequest.GrantType = request.Grant_Type;
            Tracing.Information("Grant type: " + validatedRequest.GrantType);

            // validate client credentials
            var client = ValidateClient(ClaimsPrincipal.Current, validatedRequest.Application);
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

            // validate grant types against client configuration
            if (request.Grant_Type.Equals(OAuthConstants.GrantTypes.AuthorizationCode))
            {
                if (validatedRequest.Client.Flow != OAuthFlow.Code)
                {
                    throw new TokenRequestValidationException(
                        "Code flow not allowed for client",
                        OAuthConstants.Errors.UnsupportedGrantType);
                }
            }

            if (request.Grant_Type.Equals(OAuthConstants.GrantTypes.Password))
            {
                if (validatedRequest.Client.Flow != OAuthFlow.ResourceOwner)
                {
                    throw new TokenRequestValidationException(
                        "Resource owner password flow not allowed for client",
                        OAuthConstants.Errors.UnsupportedGrantType);
                }
            }

            if (request.Grant_Type.Equals(OAuthConstants.GrantTypes.ClientCredentials))
            {
                if (validatedRequest.Client.Flow != OAuthFlow.Client)
                {
                    throw new TokenRequestValidationException(
                        "Client flow not allowed for client",
                        OAuthConstants.Errors.UnsupportedGrantType);
                }
            }

            if (request.Grant_Type.Equals(OAuthConstants.GrantTypes.RefreshToken))
            {
                if (!validatedRequest.Client.AllowRefreshToken)
                {
                    throw new TokenRequestValidationException(
                        "Refresh tokens not allowed for client",
                        OAuthConstants.Errors.UnsupportedGrantType);
                }
            }

            // resource owner password flow
            if (validatedRequest.GrantType.Equals(OAuthConstants.GrantTypes.Password))
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

            Tracing.Information("Token request validation successful.");
            return validatedRequest;
        }

        private Client ValidateClient(ClaimsPrincipal clientPrincipal, Application application)
        {
            if (!clientPrincipal.Identity.IsAuthenticated)
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
