using System.Collections.Generic;
using System.Linq;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer
{
    public class DummyAuthorizationServerConfiguration : IAuthorizationServerConfiguration
    {
        List<Application> _applications = new List<Application>();

        public DummyAuthorizationServerConfiguration()
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

            var CodeClient = new Client
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

            var ImplicitClient = new Client
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

            var readScope = new Scope
            {
                AllowedClients = new List<Client> { CodeClient, ImplicitClient, resourceOwnerClient },
                Name = "read",
                Description = "Read data",
                Emphasize = false
            };

            var searchScope = new Scope
            {
                AllowedClients = new List<Client> { CodeClient, resourceOwnerClient },
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

            var application = new Application
            {
                Name = "User management",
                Namespace = "users",
                Scopes = new List<Scope> { readScope, searchScope, writeScope },
                //Clients = new List<Client> { CodeClient, ImplicitClient, resourceOwnerClient },
                RequireConsent = true,
                TokenLifetime = 60
            };

            _applications.Add(application);
        }
    }
}
