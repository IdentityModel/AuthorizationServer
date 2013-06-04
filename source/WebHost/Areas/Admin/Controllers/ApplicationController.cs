
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Controllers
{
    public class ApplicationController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult Scopes()
        {
            return View();
        }
        
        public ActionResult Scope()
        {
            return View();
        }

        public ActionResult ScopeClients()
        {
            return View();
        }

    }
}
