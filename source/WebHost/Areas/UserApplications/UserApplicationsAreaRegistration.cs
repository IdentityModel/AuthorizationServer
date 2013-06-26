using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.UserApplications
{
    public class UserApplicationsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "UserApplications";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "UserApplications_default",
                "UserApplications/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            RegisterBundles(BundleTable.Bundles);
            RegisterWebApiRoutes(GlobalConfiguration.Configuration);
        }

        private void RegisterWebApiRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "UserApps-Endpoints",
                routeTemplate: "api/UserApplications/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/UserApplications").Include(
                "~/Areas/UserApplications/Scripts/UserApplications.js"));

            bundles.Add(new StyleBundle("~/Content/UserApplications").Include(
                "~/Areas/UserApplications/Content/UserApplications.css"));
        }

    }
}
