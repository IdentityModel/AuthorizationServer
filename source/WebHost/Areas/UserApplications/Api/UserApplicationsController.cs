/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.UserApplications.Api
{
    [Authorize]
    [ValidateHttpAntiForgeryToken]
    public class UserApplicationsController : ApiController
    {
        IAuthorizationServerAdministration config;

        public UserApplicationsController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            var subject = ClaimsPrincipal.Current.GetSubject();
            if (String.IsNullOrWhiteSpace(subject)) return Request.CreateResponse(HttpStatusCode.NotFound);

            var query =
                from token in config.Tokens.All
                where
                    token.Type != AuthorizationServer.Models.StoredGrantType.AuthorizationCode &&
                    token.Subject == subject
                select new { id=token.Client.ClientId, name=token.Client.Name, application=token.Application.Name };
            var tokens = query.ToArray();
            var data =
                from token in tokens
                group token by token.id into clients
                select new { 
                    id = clients.Key, 
                    name = clients.First().name, 
                    apps = clients.Select(x => x.application).Distinct() 
                };
            return Request.CreateResponse(HttpStatusCode.OK, data.ToArray());
        }

        public HttpResponseMessage Delete(string id)
        {
            var subject = ClaimsPrincipal.Current.GetSubject();
            if (String.IsNullOrWhiteSpace(subject)) return Request.CreateResponse(HttpStatusCode.NotFound);

            var query =
                from token in this.config.Tokens.All
                where token.Subject == subject && token.Client.ClientId == id
                select token;
            foreach (var item in query)
            {
                this.config.Tokens.Remove(item);
            }
            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        public HttpResponseMessage Delete()
        {
            var subject = ClaimsPrincipal.Current.GetSubject();
            if (String.IsNullOrWhiteSpace(subject)) return Request.CreateResponse(HttpStatusCode.NotFound);

            var query =
                from token in this.config.Tokens.All
                where token.Subject == subject
                select token;
            foreach (var item in query)
            {
                this.config.Tokens.Remove(item);
            }
            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
