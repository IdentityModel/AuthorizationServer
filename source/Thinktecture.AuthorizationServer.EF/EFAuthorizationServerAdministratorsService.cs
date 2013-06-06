using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
