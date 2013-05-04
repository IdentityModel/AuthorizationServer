using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Core.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class AuthorizeController : Controller
    {
        // GET /oauth/{appName}/authorize
        //
        public ActionResult Index(string appName, AuthorizeRequest request)
        {
            Tracing.Start("OAuth2 Authorize Endoint");

            ValidatedAuthorizeRequest validRequest;
            var error = ValidateAuthorizationRequest(appName, request, out validRequest);
            if (error != null)
            {
                Tracing.Error("Aborting OAuth2 authorization request");
                return error;
            }

            if (validRequest.Application.ShowConsent)
            {
                // show consent screen
                return View("Consent");
            }

            return ProcessRequest(validRequest);
        }

        private ActionResult ProcessRequest(ValidatedAuthorizeRequest validRequest)
        {
            throw new NotImplementedException();
        }

        private ActionResult ValidateAuthorizationRequest(string appName, AuthorizeRequest request, out ValidatedAuthorizeRequest validRequest)
        {
            validRequest = new ValidatedAuthorizeRequest();
            
            // validate request model binding
            if (request == null || string.IsNullOrWhiteSpace(appName))
            {
                ViewBag.Message = "Invalid request parameters.";
                Tracing.Error(ViewBag.Message);
                return View("Error");
            }

            // validate appName
            var application = (from a in Configuration.Applications
                               where a.Namespace.Equals(appName)
                               select a)
                              .FirstOrDefault();

            if (application == null)
            {
                Tracing.Error("Application not found: " + appName);
                return HttpNotFound();
            }

            validRequest.Application = application;
            Tracing.InformationFormat("OAuth2 application: {0} ({1})", 
                validRequest.Application.Name, 
                validRequest.Application.Namespace);
            
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

            var client = validRequest.Application.Clients.Get(request.client_id);
            if (client == null)
            {
                ViewBag.Message = "Invalid client: " + request.client_id;
                Tracing.Error(ViewBag.Message);
                
                return View("Error");
            }

            validRequest.Client = client;
            Tracing.InformationFormat("Client: {0} ({1})", 
                validRequest.Client.Name,
                validRequest.Client.ClientId);

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
                var validUri = validRequest.Client.RedirectUris.Get(request.redirect_uri);
                
                if (validUri == null)
                {
                    ViewBag.Message = "Invalid redirect URI: " + request.redirect_uri;
                    Tracing.Error(ViewBag.Message);

                    return View("Error");
                }

                validRequest.RedirectUri = validUri;
                Tracing.InformationFormat("Redirect URI: {0} ({1})",
                    validRequest.RedirectUri.Uri,
                    validRequest.RedirectUri.Description);
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
                return ClientError(new Uri(validRequest.RedirectUri.Uri), OAuthConstants.Errors.InvalidRequest, string.Empty, request.state);
            }

            // check response type (only code and token are supported)
            if (!request.response_type.Equals(OAuthConstants.ResponseTypes.Token, StringComparison.Ordinal) &&
                !request.response_type.Equals(OAuthConstants.ResponseTypes.Code, StringComparison.Ordinal))
            {
                Tracing.Error("response_type is not token or code: " + request.response_type);
                return ClientError(new Uri(validRequest.RedirectUri.Uri), OAuthConstants.Errors.UnsupportedResponseType, string.Empty, request.state);
            }

            // make sure response type allowed for this client
            if (validRequest.Client.Flow == OAuthFlows.Code &&
                request.response_type != OAuthConstants.ResponseTypes.Code)
            {
                Tracing.ErrorFormat("response_type {0} is not allowed", request.response_type);
                return ClientError(new Uri(validRequest.RedirectUri.Uri), OAuthConstants.Errors.UnsupportedResponseType, request.response_type, request.state);
            }

            if (validRequest.Client.Flow == OAuthFlows.Implicit &&
                request.response_type != OAuthConstants.ResponseTypes.Token)
            {
                Tracing.ErrorFormat("response_type {0} is not allowed", request.response_type);
                return ClientError(new Uri(validRequest.RedirectUri.Uri), OAuthConstants.Errors.UnsupportedResponseType, request.response_type, request.state);
            }

            validRequest.ResponseType = request.response_type;

            // validate scopes
            if (string.IsNullOrEmpty(request.scope))
            {
                Tracing.Error("Missing scope.");
                return ClientError(new Uri(validRequest.RedirectUri.Uri), OAuthConstants.Errors.InvalidScope, validRequest.ResponseType, request.state);
            }

            var requestedScopes = request.scope.Split(' ').ToList();
            List<Scope> resultingScopes;

            if (validRequest.Application.Scopes.TryValidateScopes(validRequest.Client.ClientId, requestedScopes, out resultingScopes))
            {
                validRequest.Scopes = resultingScopes;
                Tracing.InformationFormat("Request scopes: {0}", resultingScopes);
            }
            else
            {
                return ClientError(new Uri(validRequest.RedirectUri.Uri), OAuthConstants.Errors.InvalidScope, validRequest.ResponseType, request.state);
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

            return new RedirectResult(url);
        }
    }
}
