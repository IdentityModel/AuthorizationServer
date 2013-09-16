using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Thinktecture.IdentityModel.Clients;
using Thinktecture.IdentityModel.Extensions;

namespace Thinktecture.Samples
{
    class Program
    {
        //static Uri _baseAddress = new Uri(Constants.WebHostv1BaseAddress);
        static Uri _baseAddress = new Uri(Constants.WebHostv2BaseAddress);

        static void Main(string[] args)
        {
            var token = RequestToken();
            
            CallService(token);
            //CallServiceInvalidScope(token);
        }

        private static string RequestToken()
        {
            "Requesting token.".ConsoleYellow();

            var client = new OAuth2Client(
                new Uri(Constants.AS.OAuth2TokenEndpoint),
                Constants.Clients.Client,
                Constants.Clients.ClientSecret);

            var response = client.RequestAccessTokenClientCredentials("read");

            Console.WriteLine(" access token");
            response.AccessToken.ConsoleGreen();
            
            Console.WriteLine();
            return response.AccessToken;
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

        private static void CallServiceInvalidScope(string token)
        {
            var client = new HttpClient
            {
                BaseAddress = _baseAddress
            };

            client.SetBearerToken(token);

            while (true)
            {
                "Calling service with unsufficient scope (should fail).".ConsoleYellow();

                Helper.Timer(() =>
                {
                    var response = client.PutAsync("identity", null).Result;
                    response.EnsureSuccessStatusCode();
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
