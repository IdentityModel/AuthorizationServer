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
    public class ClientRedirectsController : ApiController
    {
        IAuthorizationServerAdministration config;

        public ClientRedirectsController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get(string id)
        {
            var query =
                from item in config.Clients.All
                where item.ClientId == id
                from uri in item.RedirectUris
                select new
                {
                    uri.ID, uri.Description, uri.Uri
                };
            return Request.CreateResponse(HttpStatusCode.OK, query.ToArray());
        }

        public HttpResponseMessage Delete(int id)
        {
            var query =
                from r in config.ClientRedirects.All
                where r.ID == id
                select r;
            var item = query.SingleOrDefault();
            if (item != null)
            {
                this.config.ClientRedirects.Remove(item);
                this.config.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        //public HttpResponseMessage Post(string id, ClientRedirectModel model)
        //{
        //    if (String.IsNullOrEmpty(model.ClientSecret))
        //    {
        //        ModelState.AddModelError("model.ClientSecret", "ClientSecret is required");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    var item = new Client();
        //    item.ClientId = model.ClientId;
        //    item.ClientSecret = model.ClientSecret;
        //    item.Name = model.Name;
        //    item.Flow = model.Flow;
        //    item.AllowRefreshToken = model.AllowRefreshToken;
        //    item.RequireConsent = model.RequireConsent;

        //    this.config.Clients.Add(item);
        //    this.config.SaveChanges();

        //    var response = Request.CreateResponse(HttpStatusCode.OK, item);
        //    var url = Url.Link("Admin-Endpoints", new { controller = "Clients", id = item.ClientId });
        //    response.Headers.Location = new Uri(url);
        //    return response;
        //}
        

        //public HttpResponseMessage Put(ClientModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    var item = this.config.Clients.All.SingleOrDefault(X => X.ClientId == model.ClientId);
        //    if (item == null)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }

        //    if (!String.IsNullOrEmpty(model.ClientSecret))
        //    {
        //        item.ClientSecret = model.ClientSecret;
        //    }
        //    item.Name = model.Name;
        //    item.Flow = model.Flow;
        //    item.AllowRefreshToken = model.AllowRefreshToken;
        //    item.RequireConsent = model.RequireConsent;

        //    this.config.SaveChanges();

        //    return Request.CreateResponse(HttpStatusCode.NoContent);
        //}

        
    }
}
