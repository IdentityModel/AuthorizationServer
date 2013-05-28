using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.EF
{
    public class EFTokenHandleManager : ITokenHandleManager
    {
        AuthorizationServerContext db;

        public EFTokenHandleManager(AuthorizationServerContext db)
        {
            this.db = db;
        }

        public void Add(Models.TokenHandle handle)
        {
            db.TokenHandles.Add(handle);
            db.SaveChanges();
        }

        public Models.TokenHandle Get(string handleIdentifier)
        {
            return db.TokenHandles.Find(handleIdentifier);
        }

        public void Delete(string handleIdentifier)
        {
            var item = db.TokenHandles.Find(handleIdentifier);
            if (item != null)
            {
                db.TokenHandles.Remove(item);
                db.SaveChanges();
            }
        }
    }
}
