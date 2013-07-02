/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.WebHost.Controllers
{
    public class HomeController : Controller
    {
        IAuthorizationServerConfiguration config;
        public HomeController(IAuthorizationServerConfiguration config)
        {
            this.config = config;
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Banner()
        {
            var global = config.GlobalConfiguration;
            if (global != null)
            {
                ViewData["ServerName"] = global.AuthorizationServerName;
            }
            return PartialView("Banner");
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            return PartialView("Footer");
        }

        public JavaScriptResult Urls()
        {
            var path = Request.ApplicationPath;
            if (!path.EndsWith("/")) path += "/";
            path += "api/";

            var url = new Uri(Request.Url, path);
            var js = String.Format("if(window.as){{window.as.Service.baseUrl='{0}';}}", url);

            return base.JavaScript(js);
        }
    }
}
