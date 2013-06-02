/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Web.Mvc;
using System.Web.Routing;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "OAuth2 Authorize Endpoint",
                url: "{appName}/oauth/authorize",
                defaults: new
                {
                    controller = "Authorize",
                    action = "Index"
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Thinktecture.AuthorizationServer.WebHost.Controllers" }
            );
        }
    }
}