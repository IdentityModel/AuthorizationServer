using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models;
using Thinktecture.IdentityModel.Authorization.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Applications)]
    public class ScopesController : ApiController
    {
        IAuthorizationServerAdministration config;

        public ScopesController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get(int id)
        {
            var scope = this.config.Scopes.All.SingleOrDefault(x => x.ID == id);
            if (scope == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var data =
                new
                {
                    scope.ID, scope.Name, scope.DisplayName, scope.Description, scope.Emphasize
                };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        public HttpResponseMessage Put(int id, ScopeModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var scope = config.Scopes.All.SingleOrDefault(x => x.ID == id);
            if (scope == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            scope.DisplayName = model.DisplayName;
            scope.Description = model.Description;
            scope.Emphasize = model.Emphasize;

            config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        public HttpResponseMessage Delete(int id)
        {
            var scope = this.config.Scopes.All.SingleOrDefault(x => x.ID == id);
            if (scope != null)
            {
                this.config.Scopes.Remove(scope);
                this.config.SaveChanges();
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
