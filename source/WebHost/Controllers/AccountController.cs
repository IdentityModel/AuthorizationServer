using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Thinktecture.AuthorizationServer.WebHost.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult SignOut()
        {
            FederatedAuthentication.WSFederationAuthenticationModule.SignOut();
            return Redirect("~");
        }

    }
}
