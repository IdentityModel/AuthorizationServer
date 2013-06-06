using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
