using System;
using System.Collections.Generic;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Test
{
    class TestClientManager : IClientManager
    {
        public string Id { get; set; }

        public string Secret { get; set; }
        public string Base64EncodedSecret
        {
            get
            {
                byte[] encodedByte = System.Text.ASCIIEncoding.ASCII.GetBytes(Secret);
                string base64EncodedSecret = Convert.ToBase64String(encodedByte);
                return base64EncodedSecret;
            }
        }

        public OAuthFlow OAuthFlow { get; set; }
        public string RedirectUri { get; set; }
        public bool AllowRefreshTokens { get; set; }

        public TestClientManager()
        {
            AllowRefreshTokens = true;
        }

        public Models.Client Get(string id)
        {
            if (id != "unknown")
            {
                var redirectUris = new List<ClientRedirectUri>();
                if (RedirectUri != null)
                {
                    redirectUris.Add(new ClientRedirectUri() { Uri = RedirectUri });
                }

                return new Client()
                {
                    ClientId = Id,
                    ClientSecret = Base64EncodedSecret,
                    Flow = OAuthFlow,
                    RedirectUris = redirectUris,
                    AllowRefreshToken = AllowRefreshTokens,
                };
            }

            return null;
        }
    }
}
