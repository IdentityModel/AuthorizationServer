
namespace Thinktecture.Samples
{
    public class Constants
    {
        //
        // change the below constants to match your local system
        //

        public const string WebHostName = "web.local";
        public const string WebHostv1Path = "/rs/api/";
        public const string WebHostv2Path = "/rs2/api/";
        
        public const string WebHostv1BaseAddress = "https://" + WebHostName + WebHostv1Path;
        public const string WebHostv2BaseAddress = "https://" + WebHostName + WebHostv2Path;
       

        public const string Application = "users";
        public const string Audience = "users";

        public static class Clients
        {
            public const string CodeClient = "codeclient";
            public const string CodeClientSecret = "secret";
            public const string CodeClientRedirectUrl = "https://localhost:44303/callback";

            public const string ResourceOwnerClient = "roclient";
            public const string ResourceOwnerClientSecret = "secret";

            public const string ImplicitClient = "implicitclient";
            public const string JavaScriptImplicitClientRedirectUrl = "https://localhost:44300/callback.cshtml";

            public const string Client = "client";
            public const string ClientSecret = "secret";
        }

        public static class AS
        {
            public const string OAuth2TokenEndpoint = "https://as.local/" + Application + "/oauth/token";
            public const string OAuth2AuthorizeEndpoint = "https://as.local/" + Application + "/oauth/authorize";

            public const string IssuerName = "AS";
            public const string SigningKey = "1fTiS2clmPTUlNcpwYzd5i4AEFJ2DEsd8TcUsllmaKQ=";
        }
    }
}