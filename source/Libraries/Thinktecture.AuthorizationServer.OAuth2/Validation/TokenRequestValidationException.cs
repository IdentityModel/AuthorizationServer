using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    [Serializable]
    public class TokenRequestValidationException : Exception
    {
        public string OAuthError { get; set; }

        public TokenRequestValidationException(string message, string oauthError)
        {
            Tracing.Error(message);
            OAuthError = oauthError;
        }

        public HttpResponseMessage CreateErrorResponse(HttpRequestMessage request)
        {
            Tracing.Information("Sending error response: " + OAuthError);

            return request.CreateErrorResponse(HttpStatusCode.BadRequest,
                string.Format("{{ \"{0}\": \"{1}\" }}", OAuthConstants.Errors.Error, OAuthError));
        }
    }
}
