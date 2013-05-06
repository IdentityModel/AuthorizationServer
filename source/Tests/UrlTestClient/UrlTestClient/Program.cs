using System;
using System.Net.Http;

namespace UrlTestClient
{
    class Program
    {
        static HttpClientHandler clientHandler = new HttpClientHandler
        {
            AllowAutoRedirect = false
        };
            
        static HttpClient client = new HttpClient(clientHandler)
        {
            BaseAddress = new Uri("https://roadie/authz/oauth/users/")
        };

        static void Main(string[] args)
        {
            Console.Clear();

            TestCodeClientValid();
        }

        private static void TestCodeClientValid()
        {
            var request = "authorize?client_id=implicitclient&redirect_uri=https://test2.local&scope=read&response_type=token";
            Run(request);
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
