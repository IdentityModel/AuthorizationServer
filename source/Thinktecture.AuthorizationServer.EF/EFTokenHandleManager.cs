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
            throw new NotImplementedException();
        }

        public bool TryGet(string handleIdentifier, out Models.TokenHandle handle)
        {
            throw new NotImplementedException();
        }

        public void Delete(string handleIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
