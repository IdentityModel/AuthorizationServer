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
    [ClientCredentialsFilter]
    public class TokenController : ApiController
    {
        IResourceOwnerCredentialValidation _rocv;
        IAuthorizationServerConfiguration _config;
        IStoredGrantManager _handleManager;
        IAssertionGrantValidation _assertionGrantValidator;
        private TokenService _tokenService;

        public TokenController(
            IResourceOwnerCredentialValidation rocv, 
            IAuthorizationServerConfiguration config,
            IStoredGrantManager handleManager,
            IAssertionGrantValidation assertionGrantValidator,
            TokenService tokenService)
        {
            _rocv = rocv;
            _config = config;
            _handleManager = handleManager;
            _assertionGrantValidator = assertionGrantValidator;
            _tokenService = tokenService;
        }

        public HttpResponseMessage Post([FromUri] string appName, [FromBody] TokenRequest request)
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
                validatedRequest = new TokenRequestValidator(_handleManager).Validate(application, request, ClaimsPrincipal.Current);
            }
            catch (TokenRequestValidationException ex)
            {
                Tracing.Error("Aborting OAuth2 token request");
                return Request.CreateOAuthErrorResponse(ex.OAuthError);
            }

            switch (validatedRequest.GrantType)
            {
                case OAuthConstants.GrantTypes.Password:
                    return ProcessResourceOwnerCredentialRequest(validatedRequest);
                case OAuthConstants.GrantTypes.AuthorizationCode:
                    return ProcessAuthorizationCodeRequest(validatedRequest);
                case OAuthConstants.GrantTypes.RefreshToken:
                    return ProcessRefreshTokenRequest(validatedRequest);
                case OAuthConstants.GrantTypes.ClientCredentials:
                    return ProcessClientCredentialsRequest(validatedRequest);
                case OAuthConstants.GrantTypes.Assertion:
                    return ProcessAssertionGrant(validatedRequest);
            }

            Tracing.Error("invalid grant type: " + request.Grant_Type);
            return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.UnsupportedGrantType);
        }

        private HttpResponseMessage ProcessAssertionGrant(ValidatedRequest validatedRequest)
        {
            ClaimsPrincipal principal;

            try
            {
                Tracing.Information("Calling assertion grant handler for assertion: " + validatedRequest.Assertion);
                principal = _assertionGrantValidator.ValidateAssertion(validatedRequest);
            }
            catch (Exception ex)
            {
                Tracing.Error("Unhandled exception in assertion grant handler: " + ex.ToString());
                throw;
            }

            if (principal == null)
            {
                Tracing.Error("Assertion grant handler failed to validate assertion");
                return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.InvalidGrant);
            }

            var response = _tokenService.CreateTokenResponse(validatedRequest, principal);
            return Request.CreateTokenResponse(response);
        }

        private HttpResponseMessage ProcessClientCredentialsRequest(ValidatedRequest validatedRequest)
        {
            Tracing.Information("Processing refresh token request");

            var response = _tokenService.CreateTokenResponse(validatedRequest);
            return Request.CreateTokenResponse(response);
        }

        private HttpResponseMessage ProcessRefreshTokenRequest(ValidatedRequest validatedRequest)
        {
            Tracing.Information("Processing refresh token request");

            var response = _tokenService.CreateTokenResponse(validatedRequest.StoredGrant, _handleManager);
            return Request.CreateTokenResponse(response);
        }

        private HttpResponseMessage ProcessAuthorizationCodeRequest(ValidatedRequest validatedRequest)
        {
            Tracing.Information("Processing authorization code request");

            var response = _tokenService.CreateTokenResponse(validatedRequest.StoredGrant, _handleManager);
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
                var response = _tokenService.CreateTokenResponse(validatedRequest, principal);

                // check if refresh token is enabled for the client
                if (validatedRequest.Client.AllowRefreshToken && validatedRequest.Application.AllowRefreshToken)
                {
                    var handle = StoredGrant.CreateRefreshTokenHandle(
                        principal.GetSubject(),
                        validatedRequest.Client,
                        validatedRequest.Application,
                        principal.Claims,
                        validatedRequest.Scopes,
                        DateTime.UtcNow.AddYears(5));

                    _handleManager.Add(handle);
                    response.RefreshToken = handle.GrantId;
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
