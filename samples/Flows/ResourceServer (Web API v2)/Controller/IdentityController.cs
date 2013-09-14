using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;
using Thinktecture.Samples.Models;

namespace Thinktecture.Samples
{
    [Authorize]
    public class IdentityController : ApiController
    {
        public IEnumerable<ViewClaim> Get()
        {
            return ViewClaims.GetAll(User as ClaimsPrincipal);
        }
    }
}