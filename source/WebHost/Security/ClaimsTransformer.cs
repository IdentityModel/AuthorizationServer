using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Helpers;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class ClaimsTransformer : ClaimsAuthenticationManager
    {
        IAuthorizationServerAdministratorsService service;

        public ClaimsTransformer(IAuthorizationServerAdministratorsService svc)
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

        private Claim GetSubject(ClaimsPrincipal principal)
        {
            var nameId = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (nameId == null)
            {
                nameId = principal.FindFirst(ClaimTypes.Name);
                if (nameId == null)
                {
                    throw new InvalidOperationException("No nameidentifier claim");
                }
            }

            return new Claim(Constants.ClaimTypes.Subject, nameId.Value);
        }

        private IEnumerable<Claim> AddInternalClaims(Claim subject)
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