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
    public class EFClientManager : IClientManager
    {
        AuthorizationServerContext db;

        public EFClientManager(AuthorizationServerContext db)
        {
            this.db = db;
        }

        public Models.Client Get(string clientId)
        {
            return db.Clients
                .Where(x => x.ClientId == clientId)
                .FirstOrDefault();
        }
    }
}
