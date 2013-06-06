using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class AuthorizationServerClaimsTransformer : ClaimsAuthenticationManager
    {
        IAuthorizationServerAdministratorsService service;

        public AuthorizationServerClaimsTransformer(IAuthorizationServerAdministratorsService svc)
        {
            this.service = svc;
        }

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            var nameIDs = this.service.GetAdministratorNameIDs();

            return base.Authenticate(resourceName, incomingPrincipal);
        }
    }
}