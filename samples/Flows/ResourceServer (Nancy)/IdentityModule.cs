using Nancy;
using System.Collections.Generic;
using System.Linq;

namespace ResourceServer
{
    public class IdentityModule : NancyModule
    {
        public IdentityModule()
        {
            //this.RequiresAuthentication();

            Get["/api/identity"] = request =>
                {
                    var auth = Context.GetOwinAuthentication();
                    if (auth.User.Identity.IsAuthenticated == false)
                    {
                        return HttpStatusCode.Unauthorized;
                    }

                    var claims = new List<ViewClaim>(
                        from c in auth.User.Claims
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