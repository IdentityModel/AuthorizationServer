using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api.Formatters;
using Thinktecture.IdentityModel.Authorization.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Keys)]
    public class CertificateController : ApiController
    {
        IAuthorizationServerAdministration config;

        public CertificateController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        //protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        //{
        //    base.Initialize(controllerContext);
        //    controllerContext.Configuration.Formatters.Clear();
        //    controllerContext.Configuration.Formatters.Insert(0, new X509CertificateReferenceFormatter());
        //}

        public HttpResponseMessage Get(int id)
        {
            var item = config.Keys.All.SingleOrDefault(x => x.ID == id) as X509CertificateReference;
            if (item == null) return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, item, new X509CertificateReferenceFormatter(item.Name));
        }
    }
}
