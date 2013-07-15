using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Thinktecture.AuthorizationServer.Models
{
    public static class StoredGrantExtensions
    {
        public static IEnumerable<Claim> ToClaims(this IEnumerable<StoredGrantClaim> claims)
        {
            if (claims == null) return Enumerable.Empty<Claim>();
            return claims.Select(x => x.ToClaim());
        }

        public static Claim ToClaim(this StoredGrantClaim claim)
        {
            return new Claim(claim.Type, claim.Value);
        }

        public static IEnumerable<StoredGrantClaim> ToStoredGrantClaims(this IEnumerable<Claim> claims)
        {
            if (claims == null) return Enumerable.Empty<StoredGrantClaim>();
            return claims.Select(x => x.ToStoredGrantClaim());
        }

        public static StoredGrantClaim ToStoredGrantClaim(this Claim claim)
        {
            if (claim == null) throw new ArgumentNullException("claim");
            return new StoredGrantClaim
            {
                Type = claim.Type, Value = claim.Value
            };
        }
    }
}
