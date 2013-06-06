using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            var claims = new List<Claim>();

            claims.AddRange(FilterClaims(incomingPrincipal));
            claims.AddRange(GetInternalClaims(incomingPrincipal));

            return Principal.Create("AuthorizationServer", claims.ToArray());
        }

        private IEnumerable<Claim> GetInternalClaims(ClaimsPrincipal incomingPrincipal)
        {
            var result = new List<Claim>();
            var nameId = incomingPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            var adminNameIDs = this.service.GetAdministratorNameIDs();

            if (adminNameIDs.Contains(nameId.Value))
            {
                result.Add(new Claim(
                    ClaimTypes.Role, 
                    Constants.Roles.Administrators, 
                    ClaimValueTypes.String, 
                    Constants.InternalIssuer));
            }

            return result;
        }

        private IEnumerable<Claim> FilterClaims(ClaimsPrincipal incomingPrincipal)
        {
            var nameId = incomingPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            if (nameId == null)
            {
                throw new InvalidOperationException("No nameidentifier claim");
            }

            return new Claim[] { new Claim(Constants.ClaimTypes.Subject, nameId.Value) };
        }
    }
}