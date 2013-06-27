/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Web.Mvc;
using System.Web.Routing;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.InitialConfiguration
{
    public class InitialConfigurationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction) return;

            if (Settings.EnableInitialConfiguration)
            {
                if (!"InitialConfiguration".Equals(filterContext.RequestContext.RouteData.DataTokens["area"]))
                {
                    var svc = DependencyResolver.Current.GetService<IAuthorizationServerAdministration>();
                    if (svc.GlobalConfiguration == null)
                    {
                        var route = new RouteValueDictionary(new
                        {
                            area = "InitialConfiguration",
                            controller = "Home",
                            action = "Index"
                        });
                        filterContext.Result = new RedirectToRouteResult(route);
                    }
                }
            }
            else
            {
                var svc = DependencyResolver.Current.GetService<IAuthorizationServerAdministration>();
                if (svc.GlobalConfiguration == null)
                {
                    filterContext.Result = new ViewResult() { ViewName="Error" };
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}