using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Helpers;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public abstract class ClaimsTransformerBase : ClaimsAuthenticationManager
    {
        protected IAuthorizationServerAdministratorsService service;
        protected abstract Claim GetSubject(ClaimsPrincipal principal);
       
        public ClaimsTransformerBase(IAuthorizationServerAdministratorsService svc)
        {
            this.service = svc;
        }

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            var subject = GetSubject(incomingPrincipal);
            var claims = new List<Claim> { subject };

            claims.AddRange(AddInternalClaims(subject));

            return Principal.Create("AuthorizationServer", claims.ToArray());
        }

        protected virtual IEnumerable<Claim> AddInternalClaims(Claim subject)
        {
            var adminNameIDs = this.service.GetAdministratorNameIDs();
            var result = new List<Claim>();

            if (adminNameIDs.Contains(subject.Value))
            {
                result.Add(new Claim(
                    ClaimTypes.Role, 
                    Constants.Roles.Administrators, 
                    ClaimValueTypes.String, 
                    Constants.InternalIssuer));
            }

            return result;
        }
    }
}