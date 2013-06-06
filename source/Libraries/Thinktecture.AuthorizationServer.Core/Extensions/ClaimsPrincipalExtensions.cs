using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.AuthorizationServer
{
    public static class ClaimsPrincipalExtensions
    {
        public static IEnumerable<Claim> FilterInternalClaims(this ClaimsPrincipal principal)
        {
            return principal.FindAll(c => c.Issuer != Constants.InternalIssuer);
        }
    }
}
