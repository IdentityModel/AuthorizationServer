/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Linq;
using System.Security.Claims;

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