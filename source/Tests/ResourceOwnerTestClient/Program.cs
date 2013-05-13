using System;
using Thinktecture.IdentityModel.Clients;

namespace ResourceOwnerTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new OAuth2Client(
                new Uri("https://roadie/authz/oauth/users/token"),
                "roclient",
                "secret");

            var response = client.RequestAccessTokenUserName("bob", "abc!123", "read");
        }
    }
}
