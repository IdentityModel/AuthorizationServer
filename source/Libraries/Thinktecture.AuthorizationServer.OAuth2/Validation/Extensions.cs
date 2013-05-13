/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public static class Extensions
    {
        public static HttpResponseMessage CreateOAuthErrorResponse(this HttpRequestMessage request, string OAuthError)
        {
            Tracing.Information("Sending error response: " + OAuthError);

            return request.CreateErrorResponse(HttpStatusCode.BadRequest,
                string.Format("{{ \"{0}\": \"{1}\" }}", OAuthConstants.Errors.Error, OAuthError));
        }

        public static ActionResult AuthorizeValidationError(this Controller controller, AuthorizeRequestValidationException exception)
        {
            var roException = exception as AuthorizeRequestResourceOwnerException;
            if (roException != null)
            {
                Tracing.Error(roException.Message);

                var result = new ViewResult
                {
                    ViewName = "ValidationError",
                };

                result.ViewBag.Message = roException.Message;

                return result;
            }

            var clientException = exception as AuthorizeRequestClientException;
            if (clientException != null)
            {
                Tracing.Error(clientException.Message);
                return new ClientErrorResult(clientException.RedirectUri, clientException.Error, clientException.ResponseType, clientException.State);
            }

            throw new ArgumentException("Invalid exception type");
        }
    }
}
