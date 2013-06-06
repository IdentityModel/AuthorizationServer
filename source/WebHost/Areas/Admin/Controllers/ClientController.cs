
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Authorization.Mvc;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Controllers
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Clients)]    
    public class ClientController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult Redirects()
        {
            return View();
        }
    }
}
