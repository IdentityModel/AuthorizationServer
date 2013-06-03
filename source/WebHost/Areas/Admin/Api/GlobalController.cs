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
    public class GlobalController : ApiController
    {
        IAuthorizationServerConfiguration config;

        public GlobalController(IAuthorizationServerConfiguration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            var config = this.config.GlobalConfiguration;
            var vm = new GlobalViewModel
            {
                Name = config.AuthorizationServerName,
                Logo = config.AuthorizationServerLogoUrl,
                Issuer = config.Issuer
            };

            return Request.CreateResponse(HttpStatusCode.OK, vm);
        }

        public HttpResponseMessage Put(GlobalViewModel model)
        {
            if (!ModelState.IsValid) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            
            var config = this.config.GlobalConfiguration;
            this.config.GlobalConfiguration.AuthorizationServerName = model.Name;
            this.config.GlobalConfiguration.AuthorizationServerLogoUrl = model.Logo;
            this.config.GlobalConfiguration.Issuer = model.Issuer;
            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
