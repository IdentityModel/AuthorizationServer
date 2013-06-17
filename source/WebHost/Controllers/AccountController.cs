/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.IdentityModel.Services;
using System.Web.Mvc;

namespace Thinktecture.AuthorizationServer.WebHost.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult SignOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                FederatedAuthentication.WSFederationAuthenticationModule.SignOut();
            }
            return View();
        }

    }
}
