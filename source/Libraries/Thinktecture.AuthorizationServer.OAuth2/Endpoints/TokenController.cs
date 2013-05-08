/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class TokenController : ApiController
    {
        IResourceOwnerCredentialValidation rocv;

        public HttpResponseMessage Post(string appName, TokenRequest request)
        {
            Tracing.Start("OAuth2 Token Endpoint");

            ValidatedRequest validatedRequest;
            var error = ValidateAuthorizationRequest(appName, request, out validatedRequest);
            if (error != null)
            {
                Tracing.Error("Aborting OAuth2 token request");
                return error;
            }

            // switch over the grant type
            if (validatedRequest.GrantType.Equals(OAuthConstants.GrantTypes.Password))
            {
                return ProcessResourceOwnerCredentialRequest(validatedRequest);
            }

            //else if (tokenRequest.Grant_Type.Equals(OAuth2Constants.GrantTypes.AuthorizationCode))
            //{
            //    return ProcessAuthorizationCodeRequest(client, tokenRequest.Code, tokenType);
            //}
            //else if (string.Equals(tokenRequest.Grant_Type, OAuth2Constants.GrantTypes.RefreshToken, System.StringComparison.Ordinal))
            //{
            //    return ProcessRefreshTokenRequest(client, tokenRequest.Refresh_Token, tokenType);
            //}

            Tracing.Error("invalid grant type: " + request.Grant_Type);
            return OAuthErrorResponseMessage(OAuthConstants.Errors.UnsupportedGrantType);
        }

        private HttpResponseMessage ProcessResourceOwnerCredentialRequest(ValidatedRequest validatedRequest)
        {
            var principal = rocv.Validate(validatedRequest.UserName, validatedRequest.Password);
            if (principal.Identity.IsAuthenticated)
            {
                var sts = new TokenService();
                var response = sts.CreateToken(validatedRequest, principal);

                return Request.CreateResponse<TokenResponse>(HttpStatusCode.OK, response);
            }
            else
            {
                return OAuthErrorResponseMessage(OAuthConstants.Errors.InvalidGrant);
            }
        }

        private HttpResponseMessage ValidateAuthorizationRequest(string appName, TokenRequest request, out ValidatedRequest validatedRequest)
        {
            validatedRequest = new ValidatedRequest();

            // validate request model binding
            if (request == null || string.IsNullOrWhiteSpace(appName))
            {
                Tracing.Error("Invalid request parameters.");
                return OAuthErrorResponseMessage(OAuthConstants.Errors.InvalidRequest);
            }

            // validate appName
            var application = (from a in AuthzConfiguration.Applications
                               where a.Namespace.Equals(appName)
                               select a)
                              .FirstOrDefault();

            if (application == null)
            {
                Tracing.Error("Application not found: " + appName);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found.");
            }

            validatedRequest.Application = application;
            Tracing.InformationFormat("OAuth2 application: {0} ({1})",
                validatedRequest.Application.Name,
                validatedRequest.Application.Namespace);

            // grant type is required
            if (string.IsNullOrWhiteSpace(request.Grant_Type))
            {
                return OAuthErrorResponseMessage(OAuthConstants.Errors.UnsupportedGrantType);
            }

            // check supported grant types
            if (!request.Grant_Type.Equals(OAuthConstants.GrantTypes.AuthorizationCode) &&
                !request.Grant_Type.Equals(OAuthConstants.GrantTypes.Password) &&
                !request.Grant_Type.Equals(OAuthConstants.GrantTypes.RefreshToken) &&
                !request.Grant_Type.Equals(OAuthConstants.GrantTypes.ClientCredentials))
            {
                return OAuthErrorResponseMessage(OAuthConstants.Errors.UnsupportedGrantType);
            }

            validatedRequest.GrantType = request.Grant_Type;

            // validate client credentials
            var client = ValidateClient(validatedRequest.Application);
            if (client == null)
            {
                Tracing.Error("Invalid client: " + ClaimsPrincipal.Current.Identity.Name);
                return OAuthErrorResponseMessage(OAuthConstants.Errors.InvalidClient);
            }

            validatedRequest.Client = client;
            Tracing.InformationFormat("Client: {0} ({1})",
                validatedRequest.Client.Name,
                validatedRequest.Client.ClientId);

            // resource owner password flow
            if (request.Grant_Type.Equals(OAuthConstants.GrantTypes.Password))
            {
                // validate scope
                if (string.IsNullOrWhiteSpace(request.Scope))
                {
                    Tracing.Error("Missing scope");
                    return OAuthErrorResponseMessage(OAuthConstants.Errors.InvalidScope);
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
                    return OAuthErrorResponseMessage(OAuthConstants.Errors.InvalidScope);
                }

                // extract username and password
                if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                {
                    Tracing.Error("missing Username or password.");
                    return OAuthErrorResponseMessage(OAuthConstants.Errors.InvalidGrant);
                }
                else
                {
                    validatedRequest.UserName = request.UserName;
                    validatedRequest.Password = request.Password;
                }
            }

            // validate grant types against client configuration
            if (request.Grant_Type.Equals(OAuthConstants.GrantTypes.AuthorizationCode))
            {
                if (validatedRequest.Client.Flow != OAuthFlows.Code)
                {
                    Tracing.Error("Code flow not allowed for client");
                    return OAuthErrorResponseMessage(OAuthConstants.Errors.UnsupportedGrantType);
                }
            }

            if (request.Grant_Type.Equals(OAuthConstants.GrantTypes.Password))
            {
                if (validatedRequest.Client.Flow != OAuthFlows.ResourceOwner)
                {
                    Tracing.Error("Resource owner password flow not allowed for client");
                    return OAuthErrorResponseMessage(OAuthConstants.Errors.UnsupportedGrantType);
                }
            }

            if (request.Grant_Type.Equals(OAuthConstants.GrantTypes.ClientCredentials))
            {
                if (validatedRequest.Client.Flow != OAuthFlows.Client)
                {
                    Tracing.Error("Client flow not allowed for client");
                    return OAuthErrorResponseMessage(OAuthConstants.Errors.UnsupportedGrantType);
                }
            }

            if (request.Grant_Type.Equals(OAuthConstants.GrantTypes.RefreshToken))
            {
                if (!validatedRequest.Client.AllowRefreshToken)
                {
                    Tracing.Error("Refresh tokens not allowed for client");
                    return OAuthErrorResponseMessage(OAuthConstants.Errors.UnsupportedGrantType);
                }
            }

            return null;
        }

        private Client ValidateClient(Application application)
        {
            if (!ClaimsPrincipal.Current.Identity.IsAuthenticated)
            {
                Tracing.Error("Anonymous client.");
                return null;
            }

            var passwordClaim = ClaimsPrincipal.Current.FindFirst("password");
            if (passwordClaim == null)
            {
                Tracing.Error("No client secret provided.");
                return null;
            }

            return application.Clients.ValidateClient(
                ClaimsPrincipal.Current.Identity.Name,
                passwordClaim.Value);
        }



        private HttpResponseMessage OAuthErrorResponseMessage(string error)
        {
            Tracing.Information("Sending error response: " + error);

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                string.Format("{{ \"{0}\": \"{1}\" }}", OAuthConstants.Errors.Error, error));
        }
    }
}
