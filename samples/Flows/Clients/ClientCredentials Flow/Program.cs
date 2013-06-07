using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Clients;
using Thinktecture.IdentityModel.Constants;
using Thinktecture.IdentityModel.Extensions;

namespace Thinktecture.Samples
{
    class Program
    {
        static Uri _baseAddress = new Uri(Constants.WebHostv1BaseAddress);

        static void Main(string[] args)
        {
            var token = RequestToken();
            CallService(token);
        }

        private static string RequestToken()
        {
            "Requesting token.".ConsoleYellow();

            var parameters = new Dictionary<string, string>
            {
                { OAuth2Constants.GrantType, OAuth2Constants.GrantTypes.ClientCredentials },
                { OAuth2Constants.Scope, "read" }
            };

            var client = new HttpClient {
                BaseAddress = new Uri(Constants.AuthzSrv.OAuth2TokenEndpoint)
            };

            client.SetBasicAuthentication(Constants.Clients.Client, Constants.Clients.ClientSecret);

            var response = client.PostAsync("", new FormUrlEncodedContent(parameters)).Result;
            response.EnsureSuccessStatusCode();

            var tokenResponse = response.Content.ReadAsAsync<TokenResponse>().Result;

            Console.WriteLine(" access token");
            tokenResponse.AccessToken.ConsoleGreen();
            
            Console.WriteLine();
            return tokenResponse.AccessToken;
        }

        private static void CallService(string token)
        {
            var client = new HttpClient {
                BaseAddress = _baseAddress
            };

            client.SetBearerToken(token);

            while (true)
            {
                "Calling service.".ConsoleYellow();

                Helper.Timer(() =>
                {
                    var response = client.GetAsync("identity").Result;
                    response.EnsureSuccessStatusCode();

                    var claims = response.Content.ReadAsAsync<IEnumerable<ViewClaim>>().Result;
                    Helper.ShowConsole(claims);
                });

                Console.ReadLine();
            }
        }
    }

    [JsonObject]
    public class TokenResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}
