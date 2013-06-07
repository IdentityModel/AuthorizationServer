using System;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Extensions;
using Windows.Security.Authentication.Web;

namespace Thinktecture.IdentityModel.WinRT
{
    public static class WebAuthentication
    {
        public async static Task<TokenResponse> DoImplicitFlowAsync(Uri endpoint, string clientId, string scope)
        {
            return await DoImplicitFlowAsync(endpoint, clientId, scope, WebAuthenticationBroker.GetCurrentApplicationCallbackUri());
        }

        public async static Task<TokenResponse> DoImplicitFlowAsync(Uri endpoint, string clientId, string scope, Uri callbackUri)
        {
            var startUri = new Uri(string.Format("{0}?client_id={1}&scope={2}&redirect_uri={3}&response_type=token",
                endpoint.AbsoluteUri,
                Uri.EscapeDataString(clientId),
                Uri.EscapeDataString(scope),
                Uri.EscapeDataString(callbackUri.AbsoluteUri)));

            try
            {
                var result = await WebAuthenticationBroker.AuthenticateAsync(
                        WebAuthenticationOptions.None,
                        startUri);

                if (result.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    return ParseImplicitResponse(result.ResponseData);
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
                //
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //
                throw;
            }
        }

        private static TokenResponse ParseImplicitResponse(string tokenResponse)
        {
            var response = new TokenResponse();
            var fragments = tokenResponse.Split('#');
            var qparams = fragments[1].Split('&');

            foreach (var param in qparams)
            {
                var parts = param.Split('=');
                if (parts.Length == 2)
                {
                    if (parts[0].Equals("access_token", StringComparison.Ordinal))
                    {
                        response.AccessToken = parts[1];

                    }
                    else if (parts[0].Equals("expires_in", StringComparison.Ordinal))
                    {
                        var expiresIn = int.Parse(parts[1]);
                        var expiresInDateTime = DateTime.UtcNow.AddSeconds(expiresIn);
                        var epoch = expiresInDateTime.ToEpochTime();

                        response.ExpiresIn = (int)epoch;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Malformed token response.");
                }
            }

            response.TokenType = "Bearer";
            return response;
        }
    }
}