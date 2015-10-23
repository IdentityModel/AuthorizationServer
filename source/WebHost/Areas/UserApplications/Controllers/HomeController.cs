/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Web.Mvc;
using Thinktecture.AuthorizationServer.WebHost.Areas.UserApplications.Models;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.UserApplications.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var vm = new UserApplicationsViewModel();
            return View(vm);
        }
    }
}
