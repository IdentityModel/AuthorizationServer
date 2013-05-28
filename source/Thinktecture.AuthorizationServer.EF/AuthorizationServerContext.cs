using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.EF
{
    public class AuthorizationServerContext : DbContext
    {
        public AuthorizationServerContext()
            : base("AuthorizationServerContext")
        {
        }

        public DbSet<GlobalConfiguration> GlobalConfiguration { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<SigningKey> SigningKeys { get; set; }
        public DbSet<TokenHandle> TokenHandles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().HasMany(x => x.RedirectUris).WithRequired();
            modelBuilder.Entity<Application>().HasMany(x => x.Scopes).WithRequired();
            modelBuilder.Entity<Scope>().HasMany(x => x.AllowedClients).WithMany();
            modelBuilder.Entity<TokenHandle>().HasMany(x => x.Scopes).WithMany();
        }
    }
}
