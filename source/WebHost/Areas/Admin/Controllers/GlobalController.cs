
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.IdentityModel.Authorization.Mvc;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Controllers
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Global)]
    public class GlobalController : Controller
    {
   
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Administrators()
        {
            return View();
        }
    }
}
