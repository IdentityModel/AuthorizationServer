/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Web.Mvc;
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
