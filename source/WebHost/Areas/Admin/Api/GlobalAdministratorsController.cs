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
    public class GlobalAdministratorsController : ApiController
    {
        IAuthorizationServerConfiguration config;

        public GlobalAdministratorsController(IAuthorizationServerConfiguration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            var config = this.config.GlobalConfiguration;
            var admins = config.Administrators.ToArray();
            return Request.CreateResponse(HttpStatusCode.OK, admins);
        }

        public HttpResponseMessage Post([FromBody] string nameID)
        {
            if (String.IsNullOrEmpty(nameID))
            {
                ModelState.AddModelError("nameID", "Invalid nameID");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var item = new AuthorizationServer.Models.AuthorizationServerAdministrator { NameID = nameID };
            this.config.GlobalConfiguration.Administrators.Add(item);
            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, item);
        }

        public HttpResponseMessage Delete(int id)
        {
            var item =
                this.config.GlobalConfiguration.Administrators.Where(x => x.ID == id).SingleOrDefault();
            if (item != null)
            {
                this.config.GlobalConfiguration.Administrators.Remove(item);
                this.config.SaveChanges();
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
