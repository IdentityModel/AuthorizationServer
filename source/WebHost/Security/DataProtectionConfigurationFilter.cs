using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Thinktecture.AuthorizationServer.WebHost.Security
{
    public class DataProtectionConfigurationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction) return;

            if (DataProtectection.Instance == null)
            {
                var vr = new ViewResult() 
                {
                    ViewName = "Error"
                };
                vr.ViewData["ErrorDetails"] = "DataProtectection has not been configured. This is required and must be done by an administrator.";
                filterContext.Result = vr;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}