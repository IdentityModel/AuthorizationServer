using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.IdentityModel.Authorization.WebApi;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Global)]
    public class GlobalAdministratorsController : ApiController
    {
        IAuthorizationServerAdministration config;

        public GlobalAdministratorsController(IAuthorizationServerAdministration config)
        {
            this.config = config;
        }

        public HttpResponseMessage Get()
        {
            var config = this.config.GlobalConfiguration;
            var admins = config.Administrators.ToArray();
            return Request.CreateResponse(HttpStatusCode.OK, admins);
        }

        public HttpResponseMessage Post([FromBody] string nameID)
        {
            if (String.IsNullOrEmpty(nameID))
            {
                ModelState.AddModelError("nameID", "Invalid nameID");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            if (this.config.GlobalConfiguration.Administrators.Any(x => x.NameID == nameID))
            {
                ModelState.AddModelError("", "That user is already an administrator.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.GetErrors());
            }

            var item = new AuthorizationServer.Models.AuthorizationServerAdministrator { NameID = nameID };
            this.config.GlobalConfiguration.Administrators.Add(item);
            this.config.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, item);
        }

        public HttpResponseMessage Delete(int id)
        {
            var item =
                this.config.GlobalConfiguration.Administrators.Where(x => x.ID == id).SingleOrDefault();
            if (item != null)
            {
                this.config.GlobalConfiguration.Administrators.Remove(item);
                this.config.SaveChanges();
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
