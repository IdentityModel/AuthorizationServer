/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api.Formatters;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models;
using Thinktecture.IdentityModel;
using Thinktecture.IdentityModel.Authorization.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Server)]
    public class CertificatesController : ApiController
    {
        IAuthorizationServerAdministration config;

        public CertificatesController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var query =
                    from cert in store.Certificates.Cast<X509Certificate2>()
                    select new { 
                        cert.Subject, 
                        cert.FriendlyName, 
                        cert.Thumbprint };
                return Request.CreateResponse(HttpStatusCode.OK, query.ToArray());
            }
            finally
            {
                store.Close();
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        public HttpResponseMessage Get(int id)
        {
            var item = config.Keys.All.SingleOrDefault(x => x.ID == id) as X509CertificateReference;
            if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, item, new X509CertificateReferenceFormatter(item.Name));
        }
    }
}
