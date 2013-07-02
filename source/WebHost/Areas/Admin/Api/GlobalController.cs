/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models;
using Thinktecture.IdentityModel.Authorization.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Server)]
    [ValidateHttpAntiForgeryToken]
    public class GlobalController : ApiController
    {
        IAuthorizationServerAdministration config;

        public GlobalController(IAuthorizationServerAdministration config)
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
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }
            
            var config = this.config.GlobalConfiguration;
            this.config.GlobalConfiguration.AuthorizationServerName = model.Name;
            this.config.GlobalConfiguration.AuthorizationServerLogoUrl = model.Logo;
            this.config.GlobalConfiguration.Issuer = model.Issuer;
            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
