using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;
using Thinktecture.Samples.Models;

namespace Thinktecture.Samples
{
    [Authorize]
    [EnableCors("https://localhost:44300", "*", "*")]
    public class IdentityController : ApiController
    {
        public IEnumerable<ViewClaim> Get()
        {
            return ViewClaims.GetAll(User as ClaimsPrincipal);
        }
    }
}