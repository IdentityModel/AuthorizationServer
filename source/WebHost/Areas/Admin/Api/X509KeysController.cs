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
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Keys)]
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

            var key = new X509CertificateReference();
            key.Name = model.Name;
            key.StoreName = System.Security.Cryptography.X509Certificates.StoreName.My;
            key.Location = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine;
            key.FindType = model.FindType == FindType.Thumbprint ?
                System.Security.Cryptography.X509Certificates.X509FindType.FindByThumbprint :
                System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectDistinguishedName;
            key.FindValue = model.Value;

            if (key.Certificate == null)
            {
                ModelState.AddModelError("", "Invalid Values For Certificate");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
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

            key.Name = model.Name;
            key.StoreName = System.Security.Cryptography.X509Certificates.StoreName.My;
            key.Location = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine;
            key.FindType = model.FindType == FindType.Thumbprint ?
                System.Security.Cryptography.X509Certificates.X509FindType.FindByThumbprint :
                System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectDistinguishedName;
            key.FindValue = model.Value;

            if (key.Certificate == null)
            {
                ModelState.AddModelError("", "Invalid Values For Certificate");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
