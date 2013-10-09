using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Thinktecture.Samples
{
    [Authorize]
    //[EnableCors("https://localhost:44300", "*", "*")]
    public class IdentityController : ApiController
    {
        public IEnumerable<Tuple<string, string>> Get()
        {
            var principal = User as ClaimsPrincipal;

            return from c in principal.Claims
                   select new Tuple<string, string>(c.Type, c.Value);
        }
    }
}