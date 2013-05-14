using System.Collections.Generic;
using System.Linq;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Test
{
    public class TestAuthorizationServerConfiguration : IAuthorizationServerConfiguration
    {
        List<Application> _applications = new List<Application>();

        public TestAuthorizationServerConfiguration()
        {
            PopulateData();
        }


        public Application FindApplication(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            var application = (from a in _applications
                               where a.Namespace.Equals(url)
                               select a)
                              .FirstOrDefault();

            return application;
        }

        private void PopulateData()
        {
            var resourceOwnerClient = new Client
            {
                Name = "Resource Owner Flow Client",
                ClientId = "roclient",
                ClientSecret = "secret",
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,
                Flow = OAuthFlow.ResourceOwner,
                AllowRefreshToken = true
            };

            var codeClient = new Client
            {
                Name = "Code Flow Client",
                ClientId = "codeclient",
                ClientSecret = "secret",
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

                AllowRefreshToken = true,
                Flow = OAuthFlow.Code,

                RedirectUris = new List<RedirectUri>
                    {
                        new RedirectUri
                        {
                            Uri = "https://prod.local",
                            Description = "Production"
                        },
                        new RedirectUri
                        {
                            Uri = "https://test.local",
                            Description = "Test"
                        }
                    }
            };

            var implicitClient = new Client
            {
                Name = "Implicit Flow Client",
                ClientId = "implicitclient",
                ClientSecret = "secret",
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

                AllowRefreshToken = false,
                Flow = OAuthFlow.Implicit,

                RedirectUris = new List<RedirectUri> 
                    {
                        new RedirectUri
                        {
                            Uri = "https://test2.local",
                            Description = "Test"
                        }
                    }
            };

            var trustedClient = new Client
            {
                Name = "Trusted Client",
                ClientId = "trustedclient",
                ClientSecret = "secret",
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

                AllowRefreshToken = false,
                Flow = OAuthFlow.ResourceOwner,
            };

            var readScope = new Scope
            {
                AllowedClients = new List<Client> { codeClient, implicitClient, resourceOwnerClient },
                Name = "read",
                Description = "Read data",
                Emphasize = false
            };

            var searchScope = new Scope
            {
                AllowedClients = new List<Client> { codeClient, resourceOwnerClient },
                Name = "search",
                Description = "Search data",
                Emphasize = false
            };

            var writeScope = new Scope
            {
                AllowedClients = new List<Client> { resourceOwnerClient },
                Name = "write",
                Description = "write data",
                Emphasize = true
            };

            var deleteScope = new Scope
            {
                AllowedClients = new List<Client> { trustedClient },
                Name = "delete",
                Description = "delete data",
                Emphasize = true
            };

            var application = new Application
            {
                Name = "Test Application",
                Namespace = "test",
                Scopes = new List<Scope> { readScope, searchScope, writeScope, deleteScope },
                Clients = new List<Client> { codeClient, implicitClient, resourceOwnerClient },
                ShowConsent = true,
                TokenLifetime = 60
            };

            _applications.Add(application);
        }
    }
}
