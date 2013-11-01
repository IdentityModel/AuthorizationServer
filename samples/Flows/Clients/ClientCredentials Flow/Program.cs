using System;
using System.Collections.Generic;
using System.Net.Http;
using Thinktecture.IdentityModel.Client;

namespace Thinktecture.Samples
{
    class Program
    {
        //static Uri _baseAddress = new Uri(Constants.WebHostv1BaseAddress);
        static Uri _baseAddress = new Uri(Constants.WebHostv2BaseAddress);

        static void Main(string[] args)
        {
            var response = RequestToken();
            
            CallService(response.AccessToken);
            //CallServiceInvalidScope(token);
        }

        private static TokenResponse RequestToken()
        {
            "Requesting token.".ConsoleYellow();

            var client = new OAuth2Client(
                new Uri(Constants.AS.OAuth2TokenEndpoint),
                Constants.Clients.Client,
                Constants.Clients.ClientSecret);

            var response = client.RequestClientCredentialsAsync("read").Result;

            Console.WriteLine(" access token");
            response.AccessToken.ConsoleGreen();
            
            Console.WriteLine();
            return response;
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
}
