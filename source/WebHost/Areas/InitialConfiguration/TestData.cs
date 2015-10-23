﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.EF;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class TestData
    {
        public static void Populate()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AuthorizationServerContext>());

            try
            {
                var db = DependencyResolver.Current.GetService<Thinktecture.AuthorizationServer.EF.AuthorizationServerContext>();

                var resourceOwnerClient = db.Clients.Find("roclient");
                var CodeClient = db.Clients.Find("codeclient");
                var ImplicitClient = db.Clients.Find("implicitclient");
                var client = db.Clients.Find("client");
                var assertionClient = db.Clients.Find("assertionclient");

                if (client == null)
                {
                    client = new Client
                    {
                        Enabled = true,
                        Name = "Client",
                        ClientId = "client",
                        Flow = OAuthFlow.Client
                    };
                    client.SetSharedSecret("secret");
                    db.Clients.Add(client);
                    db.SaveChanges();
                }

                if (assertionClient == null)
                {
                    assertionClient = new Client
                    {
                        Enabled = true,
                        Name = "Assertion Client",
                        AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,
                        ClientId = "assertionclient",
                        Flow = OAuthFlow.Assertion
                    };
                    assertionClient.SetSharedSecret("secret");
                    db.Clients.Add(assertionClient);
                    db.SaveChanges();
                }

                if (resourceOwnerClient == null)
                {
                    resourceOwnerClient = new Client
                    {
                        Enabled = true,
                        Name = "Resource Owner Flow Client",
                        ClientId = "roclient",
                        AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,
                        Flow = OAuthFlow.ResourceOwner,
                        AllowRefreshToken = true
                    };
                    resourceOwnerClient.SetSharedSecret("secret");
                    db.Clients.Add(resourceOwnerClient);
                    db.SaveChanges();
                }
                if (CodeClient == null)
                {
                    CodeClient = new Client
                    {
                        Enabled = true,
                        Name = "Code Flow Client",
                        ClientId = "codeclient",
                        AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

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
                            },
                            new ClientRedirectUri
                            {
                                Uri = "https://localhost:44303/callback",
                                Description = "Local Test"
                            }
                        }
                    };
                    CodeClient.SetSharedSecret("secret");
                    db.Clients.Add(CodeClient);
                    db.SaveChanges();
                }
                if (ImplicitClient == null)
                {
                    ImplicitClient = new Client
                    {
                        Enabled = true,
                        Name = "Implicit Flow Client",
                        ClientId = "implicitclient",
                        AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

                        AllowRefreshToken = false,
                        Flow = OAuthFlow.Implicit,

                        RedirectUris = new List<ClientRedirectUri>
                        {
                            new ClientRedirectUri
                            {
                                Uri = "https://test2.local",
                                Description = "Test"
                            },
                            new ClientRedirectUri
                            {
                                Uri = "https://localhost:44300/callback.cshtml",
                                Description = "JavaScript Callback Page"
                            },
                            new ClientRedirectUri
                            {
                                Uri = "ms-app://s-1-15-2-4224567138-2162094511-1976135278-3909242924-69295690-1380993013-1329561029/",
                                Description = "Win Store App"
                            }
                        }
                    };
                    ImplicitClient.SetSharedSecret("secret");
                    db.Clients.Add(ImplicitClient);
                    db.SaveChanges();
                }

                //if (!db.SigningKeys.Any())
                //{
                //    db.SigningKeys.Add(new X509CertificateReference
                //    {
                //        Name = "Default X509 Cert",
                //        Location = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
                //        FindValue = "CN=idsrv.local",
                //        FindType = System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectDistinguishedName,
                //        StoreName = System.Security.Cryptography.X509Certificates.StoreName.My
                //    });
                //    db.SaveChanges();
                //}

                if (!db.Applications.Any())
                {
                    var readScope = new Scope
                    {
                        AllowedClients = new List<Client> { CodeClient, ImplicitClient, resourceOwnerClient, client, assertionClient },
                        Name = "read",
                        DisplayName = "Read data",
                        Description = "Allows to read data",
                        Emphasize = false
                    };

                    var searchScope = new Scope
                    {
                        AllowedClients = new List<Client> { CodeClient, resourceOwnerClient },
                        Name = "search",
                        DisplayName = "Search data",
                        Description = "Allows to search for data",
                        Emphasize = false
                    };

                    var writeScope = new Scope
                    {
                        AllowedClients = new List<Client> { resourceOwnerClient },
                        Name = "write",
                        DisplayName = "Write data",
                        Description = "Allows to write data",
                        Emphasize = true
                    };

                    var key = new SymmetricKey { Name = "Demo signing key" };
                    key.SetValue(Convert.FromBase64String("1fTiS2clmPTUlNcpwYzd5i4AEFJ2DEsd8TcUsllmaKQ="));

                    var application = new Application
                    {
                        Enabled = true,
                        Name = "User management",
                        Namespace = "users",
                        Audience = "users",
                        Description = "This app manages your users",
                        LogoUrl = "http://en.opensuse.org/images/0/0b/Icon-user.png",
                        Scopes = new List<Scope> { readScope, searchScope, writeScope },
                        RequireConsent = true,
                        TokenLifetime = 60,
                        AllowRefreshToken = true,
                        AllowRememberConsentDecision = true,
                        SigningKey = key
                    };
                    
                    db.Applications.Add(application);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}