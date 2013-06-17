using EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Thinktecture.AuthorizationServer.Models;
//using Thinktecture.AuthorizationServer.Interfaces;
//using Thinktecture.AuthorizationServer.Models;

namespace EF
{
    public class EFAuthorizationServerConfigurationContext : DbContext
    {
        static EFAuthorizationServerConfigurationContext()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EFAuthorizationServerConfigurationContext>());
        }

        public EFAuthorizationServerConfigurationContext()
            : base("EFAuthorizationServerConfigurationContext")
        {
        }

        public DbSet<GlobalConfiguration> GlobalConfiguration { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<SigningKey> SigningKeys { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().HasMany(x => x.RedirectUris).WithRequired();
            modelBuilder.Entity<Application>().HasMany(x => x.Scopes).WithRequired();
            modelBuilder.Entity<Scope>().HasMany(x => x.AllowedClients).WithMany();
        }
    }
}

namespace ConsoleEFConfiguration
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new EFAuthorizationServerConfigurationContext())
            {
                db.Database.Delete();
            }

            using (var db = new EFAuthorizationServerConfigurationContext())
            {
                Console.WriteLine(db.Database.Connection.ConnectionString);

                if (!db.SigningKeys.Any())
                {
                    db.SigningKeys.Add(new X509CertificateReference
                    {
                        Location = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
                        FindType = System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectDistinguishedName,
                        StoreName = System.Security.Cryptography.X509Certificates.StoreName.My,
                        FindValue = "CN=sts"
                    });
                    db.SigningKeys.Add(new SymmetricKey { Value = new byte[] { 1, 2, 3, 4 } });
                    db.SaveChanges();
                }

                if (!db.Clients.Any())
                {
                    var client1 = new Client
                    {
                        ClientId = "client1",
                        Name = "Test Client1",
                        RedirectUris = new List<ClientRedirectUri>()
                        {
                            new ClientRedirectUri{ Description = "Test1", Uri = "http://test1"}
                        }
                    };
                    var client2 = new Client
                    {
                        ClientId = "client2",
                        Name = "Test Client2",
                        RedirectUris = new List<ClientRedirectUri>()
                        {
                            new ClientRedirectUri{ Description = "Test2", Uri = "http://test2"}
                        }
                    };
                    db.Clients.Add(client1);
                    db.Clients.Add(client2);
                    db.SaveChanges();
                }

                //var c1 = db.Clients.Find("client1");
                //if (c1 != null)
                //{
                //    db.Clients.Remove(c1);
                //    db.SaveChanges();
                //}

                if (!db.Applications.Any())
                {
                    var c1 = db.Clients.OrderBy(x => x.ClientId).Skip(0).Take(1).First();
                    var c2 = db.Clients.OrderBy(x => x.ClientId).Skip(1).Take(1).First();
                    var key = db.SigningKeys.First();

                    var app = new Application
                    {
                        Name = "Test App",
                        Scopes = new List<Scope>()
                        {
                            new Scope{ Name = "edit", 
                                AllowedClients = new List<Client>{c1} },
                            new Scope{ Name = "view", 
                                AllowedClients = new List<Client>{c2} },
                        },
                        SigningKey = key
                       
                    };
                    db.Applications.Add(app);
                    db.SaveChanges();
                }

                var a1 = db.Applications.First();
                if (a1 != null)
                {
                    foreach (var c in a1.Clients)
                    {
                        Console.WriteLine(c.Name);
                    }
                    if (a1.SigningKey != null)
                    {
                        var creds = a1.SigningKey.GetSigningCredentials();
                    }

                    //db.Applications.Remove(a1);
                    //db.SaveChanges();
                }

                //var client = db.Clients.First();
                //if (client != null)
                //{
                //    db.Clients.Remove(client);
                //    db.SaveChanges();
                //}
            }
        }
    }
}
