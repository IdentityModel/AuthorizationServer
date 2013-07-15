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
    public class EFStoredGrantManager : IStoredGrantManager
    {
        AuthorizationServerContext db;

        public EFStoredGrantManager(AuthorizationServerContext db)
        {
            this.db = db;
        }

        public void Add(Models.StoredGrant grant)
        {
            db.StoredGrants.Add(grant);
            db.SaveChanges();
        }

        public Models.StoredGrant Get(string grantIdentifier)
        {
            return db.StoredGrants.Find(grantIdentifier);
        }

        public void Delete(string grantIdentifier)
        {
            var item = db.StoredGrants.Find(grantIdentifier);
            if (item != null)
            {
                db.StoredGrants.Remove(item);
                db.SaveChanges();
            }
        }

        public Models.StoredGrant Find(string subject, Models.Client client, Models.Application application, IEnumerable<Scope> scopes, StoredGrantType type)
        {
            var grants = db.StoredGrants.Where(h => h.Subject == subject &&
                                                             h.Client.ClientId == client.ClientId &&
                                                             h.Application.ID == application.ID &&
                                                             h.Type == type).ToList();

            foreach (var grant in grants)
            {
                if (grant.Scopes.ScopeEquals(scopes))
                {
                    return grant;
                }
            }

            return null;
        }
    }
}
