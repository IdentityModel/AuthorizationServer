/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models;
using Thinktecture.IdentityModel.Authorization.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Server)]
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
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            var scope = config.Scopes.All.SingleOrDefault(x => x.ID == id);
            if (scope == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var query =
                (from a in this.config.Applications.All
                 from s in a.Scopes
                 where s.ID == id
                 select a);
            var app = query.Single();
            
            if (app.Scopes.Any(x=>x.Name == model.Name && x.ID != id))
            {
                ModelState.AddModelError("", "That Scope name is already in use.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
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
