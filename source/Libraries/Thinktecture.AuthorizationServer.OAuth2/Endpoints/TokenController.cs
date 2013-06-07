/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
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
        IResourceOwnerCredentialValidation _rocv;
        IAuthorizationServerConfiguration _config;
        ITokenHandleManager _handleManager;

        public TokenController(
            IResourceOwnerCredentialValidation rocv, 
            IAuthorizationServerConfiguration config,
            ITokenHandleManager handleManager)
        {
            _rocv = rocv;
            _config = config;
            _handleManager = handleManager;
        }

        public HttpResponseMessage Post(string appName, TokenRequest request)
        {
            Tracing.Start("OAuth2 Token Endpoint");

            // make sure application is registered
            var application = _config.FindApplication(appName);
            if (application == null)
            {
                Tracing.Error("Application not found: " + appName);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
            }

            // validate token request
            ValidatedRequest validatedRequest;
            try
            {
                validatedRequest = new TokenRequestValidator().Validate(application, request, ClaimsPrincipal.Current);
            }
            catch (TokenRequestValidationException ex)
            {
                Tracing.Error("Aborting OAuth2 token request");
                return Request.CreateOAuthErrorResponse(ex.OAuthError);
            }

            // switch over the grant type
            if (validatedRequest.GrantType.Equals(OAuthConstants.GrantTypes.Password))
            {
                return ProcessResourceOwnerCredentialRequest(validatedRequest);
            }
            else if (validatedRequest.GrantType.Equals(OAuthConstants.GrantTypes.AuthorizationCode))
            {
                return ProcessAuthorizationCodeRequest(validatedRequest);
            }
            else if (string.Equals(validatedRequest.GrantType, OAuthConstants.GrantTypes.RefreshToken))
            {
                return ProcessRefreshTokenRequest(validatedRequest);
            }

            Tracing.Error("invalid grant type: " + request.Grant_Type);
            return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.UnsupportedGrantType);
        }

        private HttpResponseMessage ProcessRefreshTokenRequest(ValidatedRequest validatedRequest)
        {
            Tracing.Information("Processing refresh token request");

            // check for refresh token in datastore
            var handle = _handleManager.Get(validatedRequest.RefreshToken);
            if (handle == null)
            {
                Tracing.Error("Refresh token not found: " + validatedRequest.RefreshToken);
                return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.InvalidGrant);
            }

            // check the client
            if (handle.Client.ClientId != validatedRequest.Client.ClientId)
            {
                Tracing.ErrorFormat("Client {0} is trying to refresh token from {1}.", validatedRequest.Client.ClientId, handle.Client.ClientId);
                return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.InvalidGrant);
            }

            var tokenService = new TokenService(_config.GlobalConfiguration);
            var response = tokenService.CreateTokenResponseFromRefreshToken(handle, _handleManager);

            return Request.CreateTokenResponse(response);
        }

        private HttpResponseMessage ProcessAuthorizationCodeRequest(ValidatedRequest validatedRequest)
        {
            Tracing.Information("Processing authorization code request");

            // check for authorization code in datastore
            var handle = _handleManager.Get(validatedRequest.AuthorizationCode);
            if (handle == null)
            {
                Tracing.Error("Authorization code not found: " + validatedRequest.AuthorizationCode);
                return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.InvalidGrant);
            }

            // check the client
            if (handle.Client.ClientId != validatedRequest.Client.ClientId)
            {
                Tracing.ErrorFormat("Client {0} is trying to request token using an authorization code from {1}.", validatedRequest.Client.ClientId, handle.Client.ClientId);
                return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.InvalidGrant);
            }

            // todo
            //if (!handle.RedirectUri.Equals(validatedRequest.RedirectUri))
            //{
            //    Tracing.ErrorFormat("Redirect URI in token request ({0}), does not match redirect URI from authorize request ({1})",
            //        validatedRequest.RedirectUri,
            //        handle.RedirectUri);

            //    return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.InvalidRequest);
            //}

            var tokenService = new TokenService(_config.GlobalConfiguration);
            var response = tokenService.CreateTokenResponseFromAuthorizationCode(handle, _handleManager);

            return Request.CreateTokenResponse(response);
        }

        private HttpResponseMessage ProcessResourceOwnerCredentialRequest(ValidatedRequest validatedRequest)
        {
            Tracing.Information("Processing resource owner credential request");

            ClaimsPrincipal principal;
            try
            {
                principal = _rocv.Validate(validatedRequest.UserName, validatedRequest.Password);
            }
            catch (Exception ex)
            {
                Tracing.Error("Resource owner credential validation failed: " + ex.ToString());
                throw;
            }

            if (principal != null && principal.Identity.IsAuthenticated)
            {
                var sts = new TokenService(this._config.GlobalConfiguration);
                var response = sts.CreateTokenResponse(validatedRequest, principal);

                // check if refresh token is enabled for the client
                if (validatedRequest.Client.AllowRefreshToken)
                {
                    var handle = TokenHandle.CreateRefreshTokenHandle(
                        principal.GetSubject(),
                        validatedRequest.Client,
                        validatedRequest.Application,
                        principal.Claims,
                        validatedRequest.Scopes);

                    _handleManager.Add(handle);
                    response.RefreshToken = handle.HandleId;
                }

                return Request.CreateTokenResponse(response);
            }
            else
            {
                return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.InvalidGrant);
            }
        }
    }
}
