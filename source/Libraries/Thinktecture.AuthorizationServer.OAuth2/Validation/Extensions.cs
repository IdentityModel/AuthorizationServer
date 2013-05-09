using System.Net;
using System.Net.Http;

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
    }
}
