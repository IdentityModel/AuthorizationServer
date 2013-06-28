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
using Thinktecture.IdentityModel.Authorization.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Server)]
    public class SymmetricKeysController : ApiController
    {
        IAuthorizationServerAdministration config;

        public SymmetricKeysController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            string key = IdentityModel.CryptoRandom.CreateRandomKeyString(32);
            return Request.CreateResponse(HttpStatusCode.OK, new { value=key });
        }

        public HttpResponseMessage Get(int id)
        {
            var item = config.Keys.All.SingleOrDefault(x => x.ID == id) as SymmetricKey;
            if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, new { item.ID, item.Name, Value=Convert.ToBase64String(item.Value) });
        }

        public HttpResponseMessage Post(SymmetricKeyModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            if (this.config.Keys.All.Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError("", "That Name is already in use.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            var sk = new SymmetricKey();
            sk.Name = model.Name;
            sk.Value = Convert.FromBase64String(model.Value);
            this.config.Keys.Add(sk);
            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, new { ID = sk.ID });
        }

        public HttpResponseMessage Put(int id, SymmetricKeyModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            var sk = this.config.Keys.All.SingleOrDefault(x => x.ID == id) as SymmetricKey;
            if (sk == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            if (this.config.Keys.All.Any(x => x.Name == model.Name && x.ID != id))
            {
                ModelState.AddModelError("", "That Name is already in use.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            } 
            
            sk.Name = model.Name;
            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
