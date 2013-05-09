/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public static class AuthorizationServerConfig
    {
        public static void Configure()
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

                RedirectUris = new RedirectUris 
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

                RedirectUris = new RedirectUris 
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
                AllowedClients = new Clients { CodeClient, ImplicitClient, resourceOwnerClient },
                Name = "read",
                Description = "Read data",
                Emphasize = false
            };

            var searchScope = new Scope
            {
                AllowedClients = new Clients { CodeClient, ImplicitClient, resourceOwnerClient },
                Name = "search",
                Description = "Search data",
                Emphasize = false
            };

            var writeScope = new Scope
            {
                AllowedClients = new Clients { CodeClient, resourceOwnerClient },
                Name = "write",
                Description = "write data",
                Emphasize = true
            };

            var application = new Application
            {
                Name = "User management",
                Namespace = "users",
                Scopes = new Scopes { readScope, searchScope, writeScope },
                Clients = new Clients { CodeClient, ImplicitClient, resourceOwnerClient },
                ShowConsent = true,
                TokenLifetime = 60
            };

            AuthzConfiguration.Applications = new List<Application>
            {
                application
            };
        }
    }
}