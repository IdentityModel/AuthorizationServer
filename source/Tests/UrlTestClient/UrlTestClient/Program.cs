using System;
using System.Diagnostics;
using System.Net.Http;

namespace UrlTestClient
{
    class Program
    {
        static string baseAddress = "https://roadie/authz/ndc/oauth/";

        static HttpClientHandler clientHandler = new HttpClientHandler
        {
            AllowAutoRedirect = false
        };
            
        static HttpClient client = new HttpClient(clientHandler)
        {
            BaseAddress = new Uri(baseAddress)
        };

        static void Main(string[] args)
        {
            Console.Clear();

            TestCodeClientValid();
        }

        private static void TestCodeClientValid()
        {
            var request = "authorize?client_id=ndc&redirect_uri=https://ndcoslo.com&scope=read delete&response_type=code";
            
            //Run(request);
            Start(request);
        }

        private static void Start(string request)
        {
            Process.Start(baseAddress + request);
        }

        private static void Run(string url)
        {
            var response = client.GetAsync(url).Result;
            Console.WriteLine(response.StatusCode);
            Console.WriteLine();

            if (response.StatusCode == System.Net.HttpStatusCode.Redirect)
            {
                Console.WriteLine(response.Headers.Location);
            }
            else
            {
                var body = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(body);
            }
        }
    }
}
