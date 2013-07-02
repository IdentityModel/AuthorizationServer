/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.IdentityModel.Authorization.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Server)]
    [ValidateHttpAntiForgeryToken]
    public class TokensController : ApiController
    {
        IAuthorizationServerAdministration config;

        public TokensController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            var query =
                from item in config.Tokens.All
                orderby item.Created
                select new { 
                    id = item.HandleId,
                    type = item.Type == TokenHandleType.AuthorizationCode ? "authorization" : (item.Type == TokenHandleType.RefreshTokenIdentifier ? "refresh":"consent"),
                    subject = item.Subject,
                    client = item.Client.Name,
                    created = item.Created,
                    application = item.Application.Name
                };
            return Request.CreateResponse(HttpStatusCode.OK, query.ToArray());
        }

        public HttpResponseMessage Delete(string id)
        {
            var item = this.config.Tokens.All.SingleOrDefault(x => x.HandleId == id);
            if (item != null)
            {
                this.config.Tokens.Remove(item);
                this.config.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        
        public HttpResponseMessage Delete()
        {
            foreach(var item in this.config.Tokens.All)
            {
                this.config.Tokens.Remove(item);
            }
            this.config.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
