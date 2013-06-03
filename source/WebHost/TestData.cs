using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.EF;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.IdentityModel;

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
                if (!db.GlobalConfiguration.Any())
                {
                    var config = new GlobalConfiguration
                    {
                        AuthorizationServerName = "Thinktecture AuthorizationServer",
                        Issuer = "ThinktectureAuthorizationServer",
                        Administrators = new List<AuthorizationServerAdministrator>
                            {
                                new AuthorizationServerAdministrator{NameID="dbaier"},
                                new AuthorizationServerAdministrator{NameID="ballen"},
                            }
                    };
                    db.GlobalConfiguration.Add(config);
                    db.SaveChanges();
                }

                var resourceOwnerClient = db.Clients.Find("roclient");
                var CodeClient = db.Clients.Find("codeclient");
                var ImplicitClient = db.Clients.Find("implicitclient");

                if (resourceOwnerClient == null)
                {
                    resourceOwnerClient = new Client
                    {
                        Name = "Resource Owner Flow Client",
                        ClientId = "roclient",
                        ClientSecret = "secret",
                        AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,
                        Flow = OAuthFlow.ResourceOwner,
                        AllowRefreshToken = true
                    };
                    db.Clients.Add(resourceOwnerClient);
                    db.SaveChanges();
                }
                if (CodeClient == null)
                {
                    CodeClient = new Client
                    {
                        Name = "Code Flow Client",
                        ClientId = "codeclient",
                        ClientSecret = "secret",
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
                            }
                        }
                    };
                    db.Clients.Add(CodeClient);
                    db.SaveChanges();
                }
                if (ImplicitClient == null)
                {
                    ImplicitClient = new Client
                    {
                        Name = "Implicit Flow Client",
                        ClientId = "implicitclient",
                        ClientSecret = "secret",
                        AuthenticationMethod = ClientAuthenticationMethod.SharedSecret,

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
                    db.Clients.Add(ImplicitClient);
                    db.SaveChanges();
                }

                if (!db.Applications.Any())
                {
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
                        Audience = "urn:users",
                        Scopes = new List<Scope> { readScope, searchScope, writeScope },
                        RequireConsent = true,
                        TokenLifetime = 60,
                        SigningKey = new SymmetricKey { Name="main signing key", Value = CryptoRandom.CreateRandomKey(32) }
                    };
                    db.Applications.Add(application);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal static void Test()
        {
            //var db = DependencyResolver.Current.GetService<Thinktecture.AuthorizationServer.EF.AuthorizationServerContext>();
            //var app = db.Applications.First();
            //var scopes = app.Scopes.ToArray();
        }
    }
}