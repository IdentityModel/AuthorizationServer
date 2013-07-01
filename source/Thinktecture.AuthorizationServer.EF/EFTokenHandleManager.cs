/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

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

        public Models.TokenHandle Find(string subject, Models.Client client, Models.Application application, TokenHandleType type)
        {
            var handle = db.TokenHandles.FirstOrDefault(h => h.Subject == subject &&
                                                             h.Client.ClientId == client.ClientId &&
                                                             h.Application.ID == application.ID &&
                                                             h.Type == type);

            return handle;
        }
    }
}
