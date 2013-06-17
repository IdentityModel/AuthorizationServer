/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Linq;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.EF
{
    public class EFAuthorizationServerConfiguration : IAuthorizationServerConfiguration
    {
        AuthorizationServerContext db;

        public EFAuthorizationServerConfiguration(AuthorizationServerContext db)
        {
            this.db = db;
        }

        public Models.GlobalConfiguration GlobalConfiguration
        {
            get { return db.GlobalConfiguration.Single(); }
        }

        public Models.Application FindApplication(string url)
        {
            return db.Applications.SingleOrDefault(x => x.Namespace == url);
        }
    }
}
