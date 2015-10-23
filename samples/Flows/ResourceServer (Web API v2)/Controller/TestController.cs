using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Thinktecture.IdentityModel.WebApi;

namespace Thinktecture.Samples.Controller
{
    public class TestController : ApiController
    {
        [ResourceActionAuthorize("action", "resource")]
        [ScopeAuthorize("read")]
        public string Get()
        {
            return "ok";
        }
    }
}