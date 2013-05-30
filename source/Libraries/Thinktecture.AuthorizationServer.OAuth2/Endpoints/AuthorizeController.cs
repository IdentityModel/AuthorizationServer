/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Security.Claims;
using System.Web.Mvc;
using System.Linq;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class AuthorizeController : Controller
    {
        ITokenHandleManager _handleManager;
        IAuthorizationServerConfiguration _config;

        public AuthorizeController(ITokenHandleManager handleManager, IAuthorizationServerConfiguration config)
        {
            _handleManager = handleManager;
            _config = config;
        }

        // GET /oauth/{appName}/authorize
        //
        public ActionResult Index(string appName, AuthorizeRequest request)
        {
            Tracing.Start("OAuth2 Authorize Endoint");

            // make sure application is registered
            var application = _config.FindApplication(appName);
            if (application == null)
            {
                Tracing.Error("Application not found: " + appName);
                return HttpNotFound();
            }

            ValidatedRequest validatedRequest;
            try
            {
                validatedRequest = new RequestValidator().ValidateAuthorizeRequest(application, request);
            }
            catch (AuthorizeRequestValidationException ex)
            {
                Tracing.Error("Aborting OAuth2 authorization request");
                return this.AuthorizeValidationError(ex);
            }

            if (validatedRequest.Application.RequireConsent)
            {
                // show consent screen
                Tracing.Verbose("Showing consent screen");

                return View("Consent", validatedRequest);
            }

            Tracing.Verbose("No consent configured for application");
            return PerformGrant(validatedRequest);
        }

        [ActionName("Index")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleConsentResponse(string appName, string button, AuthorizeRequest request)
        {
            Tracing.Start("OAuth2 Authorize Endoint - Consent response");

            // make sure application is registered
            var application = _config.FindApplication(appName);
            if (application == null)
            {
                Tracing.Error("Application not found: " + appName);
                return HttpNotFound();
            }

            if (button == "no")
            {
                Tracing.Information("User denies access token request.");
                return new ClientErrorResult(new Uri(request.redirect_uri), OAuthConstants.Errors.AccessDenied, request.response_type, request.state);
            }

            if (button == "yes")
            {
                Tracing.Information("User allows access token request.");

                ValidatedRequest validatedRequest;
                try
                {
                    validatedRequest = new RequestValidator().ValidateAuthorizeRequest(application, request);
                }
                catch (AuthorizeRequestValidationException ex)
                {
                    Tracing.Error("Aborting OAuth2 authorization request");
                    return this.AuthorizeValidationError(ex);
                }

                // todo: parse scopes form post and substitue scopes

                var grantResult = PerformGrant(validatedRequest);
                if (grantResult != null) return grantResult;
            }

            return new ClientErrorResult(
                new Uri(request.redirect_uri), 
                OAuthConstants.Errors.InvalidRequest, 
                request.response_type, 
                request.state);
        }

        private ActionResult PerformGrant(ValidatedRequest validatedRequest)
        {
            // implicit grant
            if (validatedRequest.ResponseType.Equals(OAuthConstants.ResponseTypes.Token, StringComparison.Ordinal))
            {
                return PerformImplicitGrant(validatedRequest);
            }

            // authorization code grant
            if (validatedRequest.ResponseType.Equals(OAuthConstants.ResponseTypes.Code, StringComparison.Ordinal))
            {
                return PerformAuthorizationCodeGrant(validatedRequest);
            }

            return null;
        }

        private ActionResult PerformAuthorizationCodeGrant(ValidatedRequest validatedRequest)
        {
            var handle = new TokenHandle(
                validatedRequest.Client.ClientId, 
                validatedRequest.RedirectUri.Uri,
                TokenHandleType.AuthorizationCode, 
                ClaimsPrincipal.Current.Claims,
                validatedRequest.Scopes);

            _handleManager.Add(handle);
            var tokenString = string.Format("code={0}", handle.HandleId);

            if (!string.IsNullOrWhiteSpace(validatedRequest.State))
            {
                tokenString = string.Format("{0}&state={1}", tokenString, Server.UrlEncode(validatedRequest.State));
            }

            var redirectString = string.Format("{0}?{1}",
                        validatedRequest.RedirectUri.Uri,
                        tokenString);

            return Redirect(redirectString);
        }

        private ActionResult PerformImplicitGrant(ValidatedRequest validatedRequest)
        {
            Tracing.Information("Performing implict grant");

            var sts = new TokenService(this._config.GlobalConfiguration);
            var response = sts.CreateToken(validatedRequest, ClaimsPrincipal.Current);

            var tokenString = string.Format("access_token={0}&token_type={1}&expires_in={2}",
                    response.AccessToken,
                    response.TokenType,
                    response.ExpiresIn);

            if (!string.IsNullOrWhiteSpace(validatedRequest.State))
            {
                tokenString = string.Format("{0}&state={1}", tokenString, Server.UrlEncode(validatedRequest.State));
            }

            var redirectString = string.Format("{0}#{1}",
                    validatedRequest.RedirectUri.Uri,
                    tokenString);

            Tracing.Information("Sending token response to redirect URI");
            return Redirect(redirectString);
        }
    }
}
