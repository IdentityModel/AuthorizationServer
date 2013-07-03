/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    [Authorize]
    public class AuthorizeController : Controller
    {
        ITokenHandleManager _handleManager;
        IAuthorizationServerConfiguration _config;

        public AuthorizeController(ITokenHandleManager handleManager, IAuthorizationServerConfiguration config)
        {
            _handleManager = handleManager;
            _config = config;
        }

        // GET /{appName}/oauth/authorize
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
                validatedRequest = new AuthorizeRequestValidator().Validate(application, request);
            }
            catch (AuthorizeRequestValidationException ex)
            {
                Tracing.Error("Aborting OAuth2 authorization request");
                return this.AuthorizeValidationError(ex);
            }

            if (validatedRequest.ShowConsent)
            {
                // todo: check first if a remembered consent decision exists
                if (validatedRequest.ResponseType == OAuthConstants.ResponseTypes.Token)
                {
                    var handle = _handleManager.Find(
                        ClaimsPrincipal.Current.GetSubject(),
                        validatedRequest.Client,
                        validatedRequest.Application,
                        validatedRequest.Scopes,
                        TokenHandleType.ConsentDecision);

                    if (handle != null)
                    {
                        Tracing.Verbose("Stored consent decision found.");
                        return PerformGrant(validatedRequest);
                    }
                }

                // show consent screen
                Tracing.Verbose("Showing consent screen");
                return View("Consent", validatedRequest);
            }

            Tracing.Verbose("No consent configured for application/client");
            return PerformGrant(validatedRequest);
        }

        [ActionName("Index")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleConsentResponse(string appName, string button, string[] scopes, AuthorizeRequest request, int? rememberDuration = null)
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
                    validatedRequest = new AuthorizeRequestValidator().Validate(application, request);
                }
                catch (AuthorizeRequestValidationException ex)
                {
                    Tracing.Error("Aborting OAuth2 authorization request");
                    return this.AuthorizeValidationError(ex);
                }

                if (scopes == null || scopes.Length == 0)
                {
                    ModelState.AddModelError("", "Please choose at least one permission.");
                    return View("Consent", validatedRequest);
                }

                // parse scopes form post and substitue scopes
                validatedRequest.Scopes.RemoveAll(x => !scopes.Contains(x.Name));

                // store consent decision if 
                //  checkbox was checked
                //  and storage is allowed 
                //  and flow == implicit
                if (validatedRequest.Application.AllowRememberConsentDecision &&
                    validatedRequest.ResponseType == OAuthConstants.ResponseTypes.Token &&
                    rememberDuration == -1)
                {
                    var handle = TokenHandle.CreateConsentDecisionHandle(
                        ClaimsPrincipal.Current.GetSubject(),
                        validatedRequest.Client,
                        validatedRequest.Application,
                        validatedRequest.Scopes);

                    _handleManager.Add(handle);

                    Tracing.Information("Consent decision stored.");
                }

                // parse refresh token lifetime if 
                // code flow is used 
                // and refresh tokens are allowed
                if (validatedRequest.RequestingRefreshToken &&
                    rememberDuration != null &&
                    validatedRequest.Client.Flow == OAuthFlow.Code)
                {
                    if (rememberDuration == -1)
                    {
                        validatedRequest.RefreshTokenExpiration = DateTime.UtcNow.AddYears(50);
                    }
                    else
                    {
                        validatedRequest.RefreshTokenExpiration = DateTime.UtcNow.AddHours(rememberDuration.Value);
                    }

                    Tracing.Information("Refresh token lifetime in hours: " + rememberDuration);
                }

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
            var handle = TokenHandle.CreateAuthorizationCode(
                validatedRequest.Client,
                validatedRequest.Application,
                validatedRequest.RedirectUri.Uri,
                ClaimsPrincipal.Current.FilterInternalClaims(),
                validatedRequest.Scopes,
                validatedRequest.RequestingRefreshToken,
                validatedRequest.RefreshTokenExpiration);

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
            var response = sts.CreateTokenResponse(validatedRequest, ClaimsPrincipal.Current);

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
