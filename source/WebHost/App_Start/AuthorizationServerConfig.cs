/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using Thinktecture.AuthorizationServer.Core.Models;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public static class AuthorizationServerConfig
    {
        public static void Configure()
        {
            var CodeClient = new Client
            {
                Name = "Code Flow Client",
                ClientId = "codeclient",
                ClientSecret = "secret",
                ClientSecretType = "shared",

                AllowRefreshToken = true,
                Flow = OAuthFlows.Code,

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
                ClientSecretType = "shared",

                AllowRefreshToken = false,
                Flow = OAuthFlows.Implicit,

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
                AllowedClients = new Clients { CodeClient, ImplicitClient },
                Name = "read",
                Description = "Read data",
                IsCritical = false
            };

            var searchScope = new Scope
            {
                AllowedClients = new Clients { CodeClient, ImplicitClient },
                Name = "search",
                Description = "Search data",
                IsCritical = false
            };

            var writeScope = new Scope
            {
                AllowedClients = new Clients { CodeClient },
                Name = "write",
                Description = "write data",
                IsCritical = true
            };

            var application = new Application
            {
                Name = "User management",
                Namespace = "users",
                Scopes = new Scopes { readScope, searchScope, writeScope },
                Clients = new Clients { CodeClient, ImplicitClient },
                ShowConsent = true,
                TokenLifetime = 60
            };

            Configuration.Applications = new List<Application>
            {
                application
            };
        }
    }
}