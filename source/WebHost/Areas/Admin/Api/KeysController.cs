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
    [ValidateHttpAntiForgeryToken]
    public class KeysController : ApiController
    {
        IAuthorizationServerAdministration config;

        public KeysController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            var query =
                from item in config.Keys.All.ToArray()
                select new { 
                    item.ID, 
                    item.Name, 
                    type = (item is X509CertificateReference ? "X509" : "Symmetric"), 
                    applicationCount=item.Applications.Count };
            return Request.CreateResponse(HttpStatusCode.OK, query.ToArray());
        }

        public HttpResponseMessage Get(int id)
        {
            var item = config.Keys.All.SingleOrDefault(x => x.ID == id);
            if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, item);
        }

        public HttpResponseMessage Post(SymmetricKeyModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            var sk = new SymmetricKey();
            sk.Name = model.Name;
            sk.Value = Convert.FromBase64String(model.Value);
            this.config.Keys.Add(sk);
            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, new { ID = sk.ID });
        }
        
        public HttpResponseMessage Delete(int id)
        {
            var item = this.config.Keys.All.SingleOrDefault(x => x.ID == id);
            if (item != null)
            {
                if (item.Applications.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict);
                }

                this.config.Keys.Remove(item);
                this.config.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

       
    }
}
