using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Thinktecture.Samples.Models
{
    public static class ViewClaims
    {
        public static IEnumerable<ViewClaim> GetAll(ClaimsPrincipal principal)
        {
            var claims = new List<ViewClaim>(
                from c in principal.Claims
                select new ViewClaim
                {
                    Type = c.Type,
                    Value = c.Value
                });

            return claims;
        }
    }
    
    public class ViewClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}