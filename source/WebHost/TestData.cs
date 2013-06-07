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
                                new AuthorizationServerAdministrator{NameID="dominick"},
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
                            },
                            new ClientRedirectUri
                            {
                                Uri = "https://localhost:44303/callback",
                                Description = "Local Test"
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
                            },
                            new ClientRedirectUri
                            {
                                Uri = "ms-app://s-1-15-2-4224567138-2162094511-1976135278-3909242924-69295690-1380993013-1329561029/",
                                Description = "Win Store App"
                            }
                        }
                    };
                    db.Clients.Add(ImplicitClient);
                    db.SaveChanges();
                }

                if (!db.SigningKeys.Any())
                {
                    db.SigningKeys.Add(new X509CertificateReference
                    {
                        Name = "Default X509 Cert",
                        Location = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
                        FindValue = "CN=idsrv.local",
                        FindType = System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectDistinguishedName,
                        StoreName = System.Security.Cryptography.X509Certificates.StoreName.My
                    });
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
                        Audience = "users",
                        Description = "This app manages your users",
                        LogoUrl = "http://en.opensuse.org/images/0/0b/Icon-user.png",
                        Scopes = new List<Scope> { readScope, searchScope, writeScope },
                        RequireConsent = true,
                        TokenLifetime = 60,
                        SigningKey = new SymmetricKey { Name="main signing key", Value = Convert.FromBase64String("1fTiS2clmPTUlNcpwYzd5i4AEFJ2DEsd8TcUsllmaKQ=") }
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
            using (var db = new Thinktecture.AuthorizationServer.EF.AuthorizationServerContext())
            {
                if (!db.TokenHandles.Any())
                {
                    var th = new TokenHandle()
                    {
                        Created = DateTime.Now,
                        Subject = "joe",
                        Application = db.Applications.First(),
                        Client = db.Clients.First(),
                        ResourceOwner = new List<TokenHandleClaim>()
                        {
                            new TokenHandleClaim{Type = "foo", Value="bar"}
                        }
                    };
                    db.TokenHandles.Add(th);
                    db.SaveChanges();
                }
            }

            //using (var db = new Thinktecture.AuthorizationServer.EF.AuthorizationServerContext())
            //{
            //    var th = db.TokenHandles.First();
            //    db.TokenHandles.Remove(th);
            //    db.SaveChanges();
            //}
        }
    }
}