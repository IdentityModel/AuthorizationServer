using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Thinktecture.AuthorizationServer
{
    public static class ClaimsPrincipalExtensions
    {
        public static IEnumerable<Claim> FilterInternalClaims(this ClaimsPrincipal principal)
        {
            return principal.FindAll(c => c.Issuer != Constants.InternalIssuer);
        }

        public static string GetSubject(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(Constants.ClaimTypes.Subject);
            if (claim == null)
            {
                throw new InvalidOperationException("No subject claim found.");
            }

            return claim.Value;
        }
    }
}
