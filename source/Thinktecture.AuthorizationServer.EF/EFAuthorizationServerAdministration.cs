using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.EF
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        DbSet<T> dbSet;

        public Repository(DbSet<T> dbSet)
        {
            this.dbSet = dbSet;
        }

        IQueryable<T> IRepository<T>.All
        {
            get { return this.dbSet; }
        }
     
        public void Add(T item)
        {
            dbSet.Add(item);
        }

        public void Remove(T item)
        {
            dbSet.Remove(item);
        }
    }

    public class EFAuthorizationServerAdministration : IAuthorizationServerAdministration
    {
        AuthorizationServerContext db;

        public EFAuthorizationServerAdministration(AuthorizationServerContext db)
        {
            this.db = db;
        }

        public Models.GlobalConfiguration GlobalConfiguration
        {
            get { return db.GlobalConfiguration.Single(); }
        }

        public IRepository<Models.Application> Applications
        {
            get { return new Repository<Models.Application>(db.Applications); }
        }

        public IRepository<Models.Client> Clients
        {
            get { return new Repository<Models.Client>(db.Clients); }
        }

        public IRepository<Models.SigningKey> Keys
        {
            get { return new Repository<Models.SigningKey>(db.SigningKeys); }
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}
