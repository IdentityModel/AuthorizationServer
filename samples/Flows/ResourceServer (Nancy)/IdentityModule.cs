using Nancy;
using System.Collections.Generic;
using System.Linq;
using Nancy.Security;

namespace ResourceServer
{
    public class IdentityModule : NancyModule
    {
        public IdentityModule()
        {
            Get["/api/identity"] = _ =>
                {
                    var principal = Context.GetOwinPrincipal();
                    
                    if (!principal.Identity.IsAuthenticated)
                    {
                        return HttpStatusCode.Unauthorized;
                    }

                    var claims = new List<ViewClaim>(
                        from c in principal.Claims
                        select new ViewClaim
                        {
                            Type = c.Type,
                            Value = c.Value
                        });

                    return Response.AsJson<List<ViewClaim>>(claims);
                };
        }
    }
}