using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Thinktecture.Samples
{
    [Authorize]
    [EnableCors("https://localhost:44300", "*", "*")]
    public class IdentityController : ApiController
    {
        public IEnumerable<ViewClaim> Get()
        {
            var principal = User as ClaimsPrincipal;

            return from c in principal.Claims
                   select new ViewClaim
                    {
                        Type = c.Type,
                        Value = c.Value
                    };
        }
    }
}