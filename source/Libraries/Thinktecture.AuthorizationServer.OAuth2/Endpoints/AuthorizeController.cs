/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class AuthorizeController : Controller
    {
        // GET /oauth/{appName}/authorize
        //
        public ActionResult Index(string appName, AuthorizeRequest request)
        {
            Tracing.Start("OAuth2 Authorize Endoint");

            ValidatedRequest validatedRequest;
            var error = ValidateAuthorizationRequest(appName, request, out validatedRequest);
            if (error != null)
            {
                Tracing.Error("Aborting OAuth2 authorization request");
                return error;
            }

            if (validatedRequest.Application.ShowConsent)
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

            if (button == "no")
            {
                Tracing.Information("User denies access token request.");
                return ClientError(new Uri(request.redirect_uri), OAuthConstants.Errors.AccessDenied, request.response_type, request.state);
            }

            if (button == "yes")
            {
                Tracing.Information("User allows access token request.");

                ValidatedRequest validatedRequest;
                var error = ValidateAuthorizationRequest(appName, request, out validatedRequest);
                if (error != null)
                {
                    Tracing.Error("Aborting OAuth2 authorization request");
                    return error;
                }

                // todo: parse scopes form post and substitue scopes

                var grantResult = PerformGrant(validatedRequest);
                if (grantResult != null) return grantResult;
            }

            return ClientError(new Uri(request.redirect_uri), OAuthConstants.Errors.InvalidRequest, request.response_type, request.state);
        }

        private ActionResult PerformGrant(ValidatedRequest validatedRequest)
        {
            // implicit grant
            if (validatedRequest.ResponseType.Equals(OAuthConstants.ResponseTypes.Token, StringComparison.Ordinal))
            {
                return PerformImplicitGrant(validatedRequest);
            }

            // authorization code grant
            //if (request.response_type.Equals(OAuth2Constants.ResponseTypes.Code, StringComparison.Ordinal))
            //{
            //    return PerformAuthorizationCodeGrant(request, client);
            //}

            return null;
        }

        private ActionResult PerformImplicitGrant(ValidatedRequest validatedRequest)
        {
            Tracing.Information("Performing implict grant");

            var sts = new TokenService();
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

        private ActionResult ValidateAuthorizationRequest(string appName, AuthorizeRequest request, out ValidatedRequest validatedRequest)
        {
            // If the request fails due to a missing, invalid, or mismatching
            // redirection URI, or if the client identifier is missing or invalid,
            // the authorization server SHOULD inform the resource owner of the
            // error and MUST NOT automatically redirect the user-agent to the
            // invalid redirection URI.

            validatedRequest = new ValidatedRequest();

            // validate request model binding
            if (request == null || string.IsNullOrWhiteSpace(appName))
            {
                ViewBag.Message = "Invalid request parameters.";
                Tracing.Error(ViewBag.Message);
                return View("Error");
            }

            // validate appName
            var application = (from a in AuthzConfiguration.Applications
                               where a.Namespace.Equals(appName)
                               select a)
                              .FirstOrDefault();

            if (application == null)
            {
                Tracing.Error("Application not found: " + appName);
                return HttpNotFound();
            }

            validatedRequest.Application = application;
            Tracing.InformationFormat("OAuth2 application: {0} ({1})",
                validatedRequest.Application.Name,
                validatedRequest.Application.Namespace);

            // make sure redirect uri is present
            if (string.IsNullOrWhiteSpace(request.redirect_uri))
            {
                ViewBag.Message = "Missing redirect URI";
                Tracing.Error(ViewBag.Message);

                return View("Error");
            }

            // validate client
            if (string.IsNullOrWhiteSpace(request.client_id))
            {
                ViewBag.Message = "Missing client identifier";
                Tracing.Error(ViewBag.Message);

                return View("Error");
            }

            var client = validatedRequest.Application.Clients.Get(request.client_id);
            if (client == null)
            {
                ViewBag.Message = "Invalid client: " + request.client_id;
                Tracing.Error(ViewBag.Message);

                return View("Error");
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
                    Tracing.Error("Redirect URI not over SSL : " + request.redirect_uri);
                    return ClientError(new Uri(request.redirect_uri), OAuthConstants.Errors.InvalidRequest, string.Empty, request.state);
                }

                // make sure redirect uri is registered with client
                var validUri = validatedRequest.Client.RedirectUris.Get(request.redirect_uri);

                if (validUri == null)
                {
                    ViewBag.Message = "Invalid redirect URI: " + request.redirect_uri;
                    Tracing.Error(ViewBag.Message);

                    return View("Error");
                }

                validatedRequest.RedirectUri = validUri;
                Tracing.InformationFormat("Redirect URI: {0} ({1})",
                    validatedRequest.RedirectUri.Uri,
                    validatedRequest.RedirectUri.Description);
            }
            else
            {
                ViewBag.Message = "Invalid redirect URI: " + request.redirect_uri;
                Tracing.Error(ViewBag.Message);

                return View("Error");
            }

            // validate response type
            if (String.IsNullOrWhiteSpace(request.response_type))
            {
                Tracing.Error("response_type is null or empty");
                return ClientError(new Uri(validatedRequest.RedirectUri.Uri), OAuthConstants.Errors.InvalidRequest, string.Empty, request.state);
            }

            // check response type (only code and token are supported)
            if (!request.response_type.Equals(OAuthConstants.ResponseTypes.Token, StringComparison.Ordinal) &&
                !request.response_type.Equals(OAuthConstants.ResponseTypes.Code, StringComparison.Ordinal))
            {
                Tracing.Error("response_type is not token or code: " + request.response_type);
                return ClientError(new Uri(validatedRequest.RedirectUri.Uri), OAuthConstants.Errors.UnsupportedResponseType, string.Empty, request.state);
            }

            // make sure response type allowed for this client
            if (validatedRequest.Client.Flow == OAuthFlows.Code &&
                request.response_type != OAuthConstants.ResponseTypes.Code)
            {
                Tracing.ErrorFormat("response_type {0} is not allowed", request.response_type);
                return ClientError(new Uri(validatedRequest.RedirectUri.Uri), OAuthConstants.Errors.UnsupportedResponseType, request.response_type, request.state);
            }

            if (validatedRequest.Client.Flow == OAuthFlows.Implicit &&
                request.response_type != OAuthConstants.ResponseTypes.Token)
            {
                Tracing.ErrorFormat("response_type {0} is not allowed", request.response_type);
                return ClientError(new Uri(validatedRequest.RedirectUri.Uri), OAuthConstants.Errors.UnsupportedResponseType, request.response_type, request.state);
            }

            validatedRequest.ResponseType = request.response_type;

            // validate scopes
            if (string.IsNullOrEmpty(request.scope))
            {
                Tracing.Error("Missing scope.");
                return ClientError(new Uri(validatedRequest.RedirectUri.Uri), OAuthConstants.Errors.InvalidScope, validatedRequest.ResponseType, request.state);
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
                return ClientError(new Uri(validatedRequest.RedirectUri.Uri), OAuthConstants.Errors.InvalidScope, validatedRequest.ResponseType, request.state);
            }

            if (!string.IsNullOrWhiteSpace(request.state))
            {
                validatedRequest.State = request.state;
                Tracing.Information("State: " + validatedRequest.State);
            }
            else
            {
                Tracing.Information("No state supplied.");
            }

            return null;
        }

        private ActionResult ClientError(Uri redirectUri, string error, string responseType, string state = null)
        {
            string url;
            string separator = "?";

            if (responseType == OAuthConstants.ResponseTypes.Token)
            {
                separator = "#";
            }

            if (string.IsNullOrEmpty(state))
            {
                url = string.Format("{0}{1}error={2}", redirectUri.AbsoluteUri, separator, error);
            }
            else
            {
                url = string.Format("{0}{1}error={2}&state={3}", redirectUri.AbsoluteUri, separator, error, Server.UrlEncode(state));
            }

            Tracing.Information("Sending back error response to client: " + url);
            return new RedirectResult(url);
        }
    }
}
