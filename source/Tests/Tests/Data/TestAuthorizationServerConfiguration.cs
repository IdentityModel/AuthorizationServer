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
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,
                
                Enabled = true,
                Flow = OAuthFlow.ResourceOwner,
                AllowRefreshToken = false
            };
            resourceOwnerClient.SetSharedSecret("secret");

            var assertionClient = new Client
            {
                Name = "Assertion Flow Client",
                ClientId = "assertionclient",
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

                Enabled = true,
                Flow = OAuthFlow.Assertion,
                AllowRefreshToken = false
            };
            assertionClient.SetSharedSecret("secret");

            var codeClient = new Client
            {
                Name = "Code Flow Client",
                ClientId = "codeclient",
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

                Enabled = true,
                AllowRefreshToken = true,
                Flow = OAuthFlow.Code,

                RedirectUris = new List<ClientRedirectUri>
                    {
                        new ClientRedirectUri
                        {
                            Uri = "https://prod.local",
                            Description = "Production"
                        },
                        new ClientRedirectUri
                        {
                            Uri = "https://test.local",
                            Description = "Test"
                        }
                    }
            };
            codeClient.SetSharedSecret("secret");

            var implicitClient = new Client
            {
                Name = "Implicit Flow Client",
                ClientId = "implicitclient",
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

                Enabled = true,
                AllowRefreshToken = false,
                Flow = OAuthFlow.Implicit,

                RedirectUris = new List<ClientRedirectUri> 
                    {
                        new ClientRedirectUri
                        {
                            Uri = "https://test2.local",
                            Description = "Test"
                        }
                    }
            };
            implicitClient.SetSharedSecret("secret");

            var trustedClient = new Client
            {
                Name = "Trusted Client",
                ClientId = "trustedclient",
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

                Enabled = true,
                AllowRefreshToken = true,
                Flow = OAuthFlow.ResourceOwner,
            };
            trustedClient.SetSharedSecret("secret");

            var serviceClient = new Client
            {
                Name = "Service Client",
                ClientId = "client",
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

                Enabled = true,
                AllowRefreshToken = false,
                Flow = OAuthFlow.Client,
            };
            serviceClient.SetSharedSecret("secret");


            var disabledClient = new Client
            {
                Name = "Disabled Client",
                ClientId = "disabledclient",
                AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

                Enabled = false,
                AllowRefreshToken = false,
                Flow = OAuthFlow.Code,

                RedirectUris = new List<ClientRedirectUri>
                {
                    new ClientRedirectUri
                    {
                        Uri = "https://prod.local",
                        Description = "Production"
                    }
                }
            };
            disabledClient.SetSharedSecret("secret");

            var readScope = new Scope
            {
                AllowedClients = new List<Client> { codeClient, implicitClient, resourceOwnerClient, serviceClient, assertionClient, disabledClient },
                Name = "read",
                Description = "Read data",
                Emphasize = false
            };

            var browseScope = new Scope
            {
                AllowedClients = new List<Client> { codeClient, implicitClient, resourceOwnerClient },
                Name = "browse",
                Description = "Browse data",
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
                Scopes = new List<Scope> { readScope, browseScope, searchScope, writeScope, deleteScope },
                RequireConsent = true,
                TokenLifetime = 60
            };
            
            _applications.Add(application);
        }

        public GlobalConfiguration GlobalConfiguration
        {
            get { return null; }
        }


        public void SaveChanges()
        {
            throw new System.NotImplementedException();
        }
    }
}
