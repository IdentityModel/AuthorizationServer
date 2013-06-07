using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thinktecture.Samples
{
    public class Constants
    {
        //
        // change the below constants to match your local system
        //

        public const string WebHostName = "roadie";
        public const string WebHostv1Path = "/rs/api/";
        public const string WebHostv1BaseAddress = "https://" + WebHostName + WebHostv1Path;
       
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

            public const string Client = "client";
            public const string ClientSecret = "secret";
        }

        public static class AuthzSrv
        {
            public const string OAuth2TokenEndpoint = "https://roadie/authz/" + Application + "/oauth/token";
            public const string OAuth2AuthorizeEndpoint = "https://roadie/authz/" + Application + "/oauth/authorize";

            public const string IssuerName = "ThinktectureAuthorizationServer";
            public const string SigningCertName = "CN=idsrv.local";
            public const string SigningKey = "1fTiS2clmPTUlNcpwYzd5i4AEFJ2DEsd8TcUsllmaKQ=";
        }
    }
}