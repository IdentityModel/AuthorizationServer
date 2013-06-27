using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.IdentityModel.Authorization.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Server)]
    public class ScopeClientsController : ApiController
    {
        IAuthorizationServerAdministration config;

        public ScopeClientsController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get(int scopeID)
        {
            var scope = this.config.Scopes.All.SingleOrDefault(x => x.ID == scopeID);
            if (scope == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var data =
                from s in scope.AllowedClients
                select new
                {
                    s.ClientId, s.Name
                };

            var clients = config.Clients.All.Select(x => new { x.ClientId, x.Name }).ToArray();
            clients = clients.Except(data).ToArray();
            return Request.CreateResponse(HttpStatusCode.OK, new { allowedClients=data.ToArray(), otherClients=clients });
        }

        public HttpResponseMessage Put(int scopeID, string clientID)
        {
            if (String.IsNullOrEmpty(clientID))
            {
                ModelState.AddModelError("clientID", "Client ID is required");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            var scope = config.Scopes.All.SingleOrDefault(x => x.ID == scopeID);
            if (scope == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var client = config.Clients.All.SingleOrDefault(x => x.ClientId == clientID);
            if (client == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            
            scope.AllowedClients.Add(client);
            config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        public HttpResponseMessage Delete(int scopeID, string clientID)
        {
            if (String.IsNullOrEmpty(clientID))
            {
                ModelState.AddModelError("clientID", "Client ID is required");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            var scope = config.Scopes.All.SingleOrDefault(x => x.ID == scopeID);
            if (scope == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var client = scope.AllowedClients.Find(x => x.ClientId == clientID);
            if (client != null)
            {
                scope.AllowedClients.Remove(client);
                config.SaveChanges();
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
