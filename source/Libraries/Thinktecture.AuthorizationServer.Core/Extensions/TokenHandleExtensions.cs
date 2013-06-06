using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.AuthorizationServer.Models
{
    public static class TokenHandleExtensions
    {
        public static IEnumerable<Claim> ToClaims(this IEnumerable<TokenHandleClaim> claims)
        {
            if (claims == null) return Enumerable.Empty<Claim>();
            return claims.Select(x => x.ToClaim());
        }

        public static Claim ToClaim(this TokenHandleClaim claim)
        {
            return new Claim(claim.Type, claim.Value);
        }

        public static IEnumerable<TokenHandleClaim> ToTokenHandleClaims(this IEnumerable<Claim> claims)
        {
            if (claims == null) return Enumerable.Empty<TokenHandleClaim>();
            return claims.Select(x => x.ToTokenHandleClaim());
        }

        public static TokenHandleClaim ToTokenHandleClaim(this Claim claim)
        {
            if (claim == null) throw new ArgumentNullException("claim");
            return new TokenHandleClaim
            {
                Type = claim.Type, Value = claim.Value
            };
        }
    }
}
