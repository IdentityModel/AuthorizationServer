﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models;
using Thinktecture.IdentityModel.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ResourceActionAuthorize(Constants.Actions.Configure, Constants.Resources.Server)]
    [ValidateHttpAntiForgeryToken]
    public class X509KeysController : ApiController
    {
        IAuthorizationServerAdministration config;

        public X509KeysController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get(int id)
        {
            var item = config.Keys.All.SingleOrDefault(x => x.ID == id) as X509CertificateReference;
            if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, new X509KeyModel(item));
        }

        public HttpResponseMessage Post(X509KeyModel model)
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

            var key = new X509CertificateReference();
            key.Name = model.Name;
            key.StoreName = System.Security.Cryptography.X509Certificates.StoreName.My;
            key.Location = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine;
            key.FindType = System.Security.Cryptography.X509Certificates.X509FindType.FindByThumbprint;
            key.FindValue = model.Thumbprint;

            var cert = key.Certificate;
            if (cert == null)
            {
                ModelState.AddModelError("", "Invalid Values For Certificate");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            try
            {
                var tmp = cert.PrivateKey;
            }
            catch (CryptographicException)
            {
                ModelState.AddModelError("", "No Read Access to Private Key of Certificate");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            if (model.FindType != FindType.Thumbprint)
            {
                key.FindType = System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectDistinguishedName;
                key.FindValue = cert.Subject;
                try
                {
                    cert = key.Certificate;
                }
                catch (InvalidOperationException)
                {
                    ModelState.AddModelError("", "Multiple certificates match that subject name");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
                }
            }

            this.config.Keys.Add(key);
            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, new X509KeyModel(key));
        }

        public HttpResponseMessage Put(int id, X509KeyModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var key = this.config.Keys.All.SingleOrDefault(x => x.ID == id) as X509CertificateReference;
            if (key == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            if (this.config.Keys.All.Any(x => x.Name == model.Name && x.ID != id))
            {
                ModelState.AddModelError("", "That Name is already in use.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            key.Name = model.Name;
            key.StoreName = System.Security.Cryptography.X509Certificates.StoreName.My;
            key.Location = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine;
            key.FindType = System.Security.Cryptography.X509Certificates.X509FindType.FindByThumbprint;
            key.FindValue = model.Thumbprint;

            var cert = key.Certificate;
            if (cert == null)
            {
                ModelState.AddModelError("", "Invalid Values For Certificate");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            try
            {
                var tmp = cert.PrivateKey;
            }
            catch (CryptographicException)
            {
                ModelState.AddModelError("", "No Read Access to Private Key of Certificate");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            if (model.FindType != FindType.Thumbprint)
            {
                key.FindType = System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectDistinguishedName;
                key.FindValue = cert.Subject;
                try
                {
                    cert = key.Certificate;
                }
                catch(InvalidOperationException)
                {
                    ModelState.AddModelError("", "Multiple certificates match that subject name");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
                }
            }

            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
