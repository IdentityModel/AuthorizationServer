/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (Settings.EnableAdmin)
            {
                context.MapRoute(
                    "Admin_default",
                    "Admin/{controller}/{action}/{id}",
                    new { action = "Index", id = UrlParameter.Optional }
                );

                RegisterAdminBundles(BundleTable.Bundles);
                RegisterWebApiRoutes(GlobalConfiguration.Configuration);
            }
        }

        private void RegisterWebApiRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "Admin-Endpoints-ScopeClient",
                routeTemplate: "api/admin/ScopeClients/{scopeID}/{clientID}",
                defaults: new { controller = "ScopeClients", clientID = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "Admin-Endpoints",
                routeTemplate: "api/admin/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private void RegisterAdminBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/admin").Include(
                "~/Areas/Admin/Content/Admin.css"));
        }
    }
}
