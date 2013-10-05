using System;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;
using Windows.Security.Authentication.Web;

namespace Thinktecture.IdentityModel.WinRT
{
    public static class WebAuthentication
    {
        public async static Task<AuthorizeResponse> DoImplicitFlowAsync(Uri endpoint, string clientId, string scope)
        {
            return await DoImplicitFlowAsync(endpoint, clientId, scope, WebAuthenticationBroker.GetCurrentApplicationCallbackUri());
        }

        public async static Task<AuthorizeResponse> DoImplicitFlowAsync(Uri endpoint, string clientId, string scope, Uri callbackUri)
        {
            var client = new OAuth2Client(endpoint);
            var startUri = client.CreateImplicitFlowUrl(
                clientId,
                scope,
                callbackUri.AbsoluteUri);

            try
            {
                var result = await WebAuthenticationBroker.AuthenticateAsync(
                        WebAuthenticationOptions.None,
                        new Uri(startUri));

                if (result.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    return new AuthorizeResponse(result.ResponseData);
                }
                else if (result.ResponseStatus == WebAuthenticationStatus.UserCancel)
                {
                    throw new Exception("User cancelled authentication");
                }
                else if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    throw new Exception("HTTP Error returned by AuthenticateAsync() : " + result.ResponseErrorDetail.ToString());
                }
                else
                {
                    throw new Exception("Error returned by AuthenticateAsync() : " + result.ResponseStatus.ToString());
                }
            }
            catch
            {
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                throw;
            }
        }
    }
}