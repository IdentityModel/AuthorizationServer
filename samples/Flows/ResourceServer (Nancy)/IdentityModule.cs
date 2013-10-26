using Nancy;
using System.Collections.Generic;
using System.Linq;
using Nancy.Security;
using ResourceServer.Security;

namespace ResourceServer
{
    public class IdentityModule : NancyModule
    {
        public IdentityModule() : base("/api/identity")
        {
            this.RequiresAuthentication();


            Get["/"] = _ =>
                {
                    var user = Context.CurrentUser as ClaimsUserIdentity;
                  
                    var claims = from c in user.Principal.Claims
                                 select new ViewClaim
                                 {
                                     Type = c.Type,
                                     Value = c.Value
                                 };

                    return Response.AsJson<IEnumerable<ViewClaim>>(claims);
                };
        }
    }
}