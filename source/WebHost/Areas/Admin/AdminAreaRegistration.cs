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
            }
        }

        private void RegisterAdminBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/admin").Include(
                "~/Areas/Admin/Scripts/Admin.js"));
            
            bundles.Add(new StyleBundle("~/Content/admin").Include(
                "~/Areas/Admin/Content/Admin.css"));
        }
    }
}
