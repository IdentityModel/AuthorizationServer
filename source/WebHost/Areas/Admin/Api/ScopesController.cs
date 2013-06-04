using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    public class ScopesController : ApiController
    {
        IAuthorizationServerAdministration config;

        public ScopesController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get(int id)
        {
            var app = this.config.Applications.All.SingleOrDefault(x => x.ID == id);
            if (app == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var data =
                from s in app.Scopes
                select new
                {
                    s.ID,s.Name,s.Description,s.Emphasize
                };
            return Request.CreateResponse(HttpStatusCode.OK, data.ToArray());
        }

        public HttpResponseMessage Post(int id, ScopeModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var app = config.Applications.All.SingleOrDefault(x => x.ID == id);
            if (app == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var scope = new Scope();
            scope.Name = model.Name;
            scope.Description = model.Description;
            scope.Emphasize = model.Emphasize;

            app.Scopes.Add(scope);
            config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, new {
                    scope.ID,
                    scope.Name,
                    scope.Description,
                    scope.Emphasize
                });
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
