using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Thinktecture.Samples
{
    public class ClaimsTransformer
    {
        public async Task<ClaimsPrincipal> Transform(ClaimsPrincipal incomingPrincipal)
        {
            if (!incomingPrincipal.Identity.IsAuthenticated)
            {
                return incomingPrincipal;
            }

            // go to datastore and add app specific claims
            incomingPrincipal.Identities.First().AddClaim(
                new Claim("localclaim", "localvalue"));

            return incomingPrincipal;
        }
    }
}