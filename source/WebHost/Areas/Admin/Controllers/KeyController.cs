/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Web.Mvc;
using Thinktecture.IdentityModel.Authorization.Mvc;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Controllers
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.Keys)]
    public class KeyController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SymmetricKey()
        {
            return View();
        }

        public ActionResult X509Key()
        {
            return View();
        }

    }
}
