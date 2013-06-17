/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Linq;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.EF
{
    public class EFAuthorizationServerAdministratorsService : IAuthorizationServerAdministratorsService
    {
        public string[] GetAdministratorNameIDs()
        {
            using (var db = new AuthorizationServerContext())
            {
                var query =
                    from a in db.GlobalConfiguration
                    from admin in a.Administrators
                    select admin.NameID;
                return query.ToArray();
            }
        }
    }
}
