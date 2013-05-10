/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
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
        IResourceOwnerCredentialValidation _rocv;
        IAuthorizationServerConfiguration _config;

        public TokenController(IResourceOwnerCredentialValidation rocv, IAuthorizationServerConfiguration config)
        {
            _rocv = rocv;
            _config = config;
        }

        public HttpResponseMessage Post(string appName, TokenRequest request)
        {
            Tracing.Start("OAuth2 Token Endpoint");

            // make sure application is registered
            var application = _config.FindApplication(appName);
            if (application == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not found");
            }

            // validate token request
            ValidatedRequest validatedRequest;
            try
            {
                validatedRequest = new RequestValidator().ValidateTokenRequest(application, request);
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

            //else if (tokenRequest.Grant_Type.Equals(OAuth2Constants.GrantTypes.AuthorizationCode))
            //{
            //    return ProcessAuthorizationCodeRequest(client, tokenRequest.Code, tokenType);
            //}
            //else if (string.Equals(tokenRequest.Grant_Type, OAuth2Constants.GrantTypes.RefreshToken, System.StringComparison.Ordinal))
            //{
            //    return ProcessRefreshTokenRequest(client, tokenRequest.Refresh_Token, tokenType);
            //}

            Tracing.Error("invalid grant type: " + request.Grant_Type);
            return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.UnsupportedGrantType);
        }

        private HttpResponseMessage ProcessResourceOwnerCredentialRequest(ValidatedRequest validatedRequest)
        {
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
                var sts = new TokenService();
                var response = sts.CreateToken(validatedRequest, principal);

                Tracing.Information("Returning token response.");
                return Request.CreateResponse<TokenResponse>(HttpStatusCode.OK, response);
            }
            else
            {
                return Request.CreateOAuthErrorResponse(OAuthConstants.Errors.InvalidGrant);
            }
        }

        //private Application GetApplication(string appName)
        //{
        //    if (string.IsNullOrWhiteSpace(appName))
        //    {
        //        return null;
        //    }

        //    var application = (from a in AuthzConfiguration.Applications
        //                       where a.Namespace.Equals(appName)
        //                       select a)
        //                      .FirstOrDefault();

        //    if (application == null)
        //    {
        //        Tracing.Error("Application not found: " + appName);
        //    }

        //    return application;
        //}
    }
}
