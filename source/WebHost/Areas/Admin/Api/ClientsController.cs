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
                select new { item.ClientId, item.Name, flow = Enum.GetName(typeof(OAuthFlow), item.Flow) };
            return Request.CreateResponse(HttpStatusCode.OK, query.ToArray());
        }
        
        public HttpResponseMessage Get(string id)
        {
            var item = config.Clients.All.SingleOrDefault(x=>x.ClientId==id);
            if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            
            var data = new { item.ClientId, item.Name, flow = Enum.GetName(typeof(OAuthFlow), item.Flow) };
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

        public HttpResponseMessage Post(ClientModel model)
        {
            var item = new Client();
            item.ClientId = model.ClientID;
            item.ClientSecret = model.ClientSecret;
            item.Name = model.Name;
            item.Flow = model.Flow;
            item.AllowRefreshToken = model.AllowRefreshToken;
            item.RequireConsent = model.RequireConsent;
            
            this.config.Clients.Add(item);
            this.config.SaveChanges();

            var response = Request.CreateResponse(HttpStatusCode.OK, item);
            var url = Url.Link("Admin-Endpoints", new { controller = "Clients", id = item.ClientId });
            response.Headers.Location = new Uri(url);
            return response;
        }
    }
}
