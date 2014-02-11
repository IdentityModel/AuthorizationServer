using Nancy;
using Nancy.Security;
using System.Collections.Generic;
using System.Linq;

namespace ResourceServer
{
    public class IdentityModule : NancyModule
    {
        public IdentityModule() : base("/api/identity")
        {
            this.RequiresMSOwinAuthentication();


            Get["/"] = _ =>
                {
                    var user = Context.GetMSOwinUser();
                  
                    var claims = from c in user.Claims
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