/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using System.Linq;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

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

        public Models.TokenHandle Find(string subject, Models.Client client, Models.Application application, IEnumerable<Scope> scopes, TokenHandleType type)
        {
            var handles = db.TokenHandles.Where(h => h.Subject == subject &&
                                                             h.Client.ClientId == client.ClientId &&
                                                             h.Application.ID == application.ID &&
                                                             h.Type == type).ToList();

            foreach (var handle in handles)
            {
                if (handle.Scopes.ScopeEquals(scopes))
                {
                    return handle;
                }
            }

            return null;
        }
    }
}
