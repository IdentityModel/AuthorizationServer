using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    public class ApplicationsController : ApiController
    {
        IAuthorizationServerAdministration config;

        public ApplicationsController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            var query =
                from a in config.Applications.All
                select new { a.ID, a.Namespace, a.Name, a.LogoUrl };
            return Request.CreateResponse(HttpStatusCode.OK, query.ToArray());
        }
        
        public HttpResponseMessage Delete(int id)
        {
            var app = this.config.Applications.All.SingleOrDefault(x => x.ID == id);
            if (app != null)
            {
                this.config.Applications.Remove(app);
                this.config.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

       
    }
}
