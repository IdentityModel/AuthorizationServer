using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            var action = context.Action.First().Value;

            switch (action)
            {
                case Constants.Actions.Configure:
                    return AuthorizeAdminArea(context);
            }

            return false;
        }

        private bool AuthorizeAdminArea(AuthorizationContext context)
        {
            return (context.Principal.IsInRole(Constants.Roles.Administrators));
        }
    }
}