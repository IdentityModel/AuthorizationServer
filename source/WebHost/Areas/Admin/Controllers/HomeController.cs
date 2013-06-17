/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Authorization.Mvc;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Controllers
{
    [ClaimsAuthorize(Constants.Actions.Configure, Constants.Resources.General)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Global");
        }

        public JavaScriptResult Urls()
        {
            var path = Request.ApplicationPath;
            if (!path.EndsWith("/")) path += "/";
            path += "api/";
            
            var url = new Uri(Request.Url, path);
            var js = String.Format("if(authz){{authz.baseUrl='{0}';}}", url);
            
            return base.JavaScript(js);
        }
    }
}
