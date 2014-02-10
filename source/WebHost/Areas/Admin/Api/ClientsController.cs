/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models;
using Thinktecture.IdentityModel.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ResourceActionAuthorize(Constants.Actions.Configure, Constants.Resources.Server)]
    [ValidateHttpAntiForgeryToken]
    public class ClientsController : ApiController
    {
        IAuthorizationServerAdministration config;

        public ClientsController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            var query =
                from item in config.Clients.All.ToArray()
                select new { 
                    item.ClientId, 
                    item.Name, 
                    flow = Enum.GetName(typeof(OAuthFlow), item.Flow),
                    item.Enabled
                };
            return Request.CreateResponse(HttpStatusCode.OK, query.ToArray());
        }
        
        public HttpResponseMessage Get(string id)
        {
            var item = config.Clients.All.SingleOrDefault(x=>x.ClientId==id);
            if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            
            var data = new { 
                item.ClientId, 
                item.Name, 
                flow = Enum.GetName(typeof(OAuthFlow), item.Flow),
                item.AllowRefreshToken, 
                item.RequireConsent,
                item.Enabled,
                clientSecret = item.GetSharedSecret()
            };
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
        
        public HttpResponseMessage Delete(string id)
        {
            var item = this.config.Clients.All.SingleOrDefault(x => x.ClientId == id);
            if (item != null)
            {
                this.config.Clients.Remove(item);
                this.config.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        public HttpResponseMessage Put(ClientModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            var item = this.config.Clients.All.SingleOrDefault(X => X.ClientId == model.ClientId);
            if (item == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (config.Clients.All.Any(x => x.Name == model.Name && x.ClientId != model.ClientId))
            {
                ModelState.AddModelError("", "That Name is already in use.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            item.Name = model.Name;
            item.Flow = model.Flow;
            item.AllowRefreshToken = model.AllowRefreshToken;
            item.RequireConsent = model.RequireConsent;
            item.Enabled = model.Enabled;
            item.SetSharedSecret(model.ClientSecret);

            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        public HttpResponseMessage Post(ClientModel model)
        {
            if (String.IsNullOrEmpty(model.ClientSecret))
            {
                ModelState.AddModelError("model.ClientSecret", "ClientSecret is required");
            }

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            if (config.Clients.All.Any(x => x.ClientId == model.ClientId))
            {
                ModelState.AddModelError("", "That Client ID is already in use.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }
            if (config.Clients.All.Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError("", "That Name is already in use.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            var item = new Client();
            item.ClientId = model.ClientId;
            item.Name = model.Name;
            item.Flow = model.Flow;
            item.AllowRefreshToken = model.AllowRefreshToken;
            item.RequireConsent = model.RequireConsent;
            item.Enabled = model.Enabled;
            item.SetSharedSecret(model.ClientSecret);
            
            this.config.Clients.Add(item);
            this.config.SaveChanges();

            var response = Request.CreateResponse(HttpStatusCode.OK, item);
            var url = Url.Link("Admin-Endpoints", new { controller = "Clients", id = item.ClientId });
            response.Headers.Location = new Uri(url);
            return response;
        }
    }
}
