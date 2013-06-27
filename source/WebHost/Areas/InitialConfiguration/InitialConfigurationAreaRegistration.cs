/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Web.Mvc;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.InitialConfiguration
{
    public class InitialConfigurationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "InitialConfiguration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            GlobalFilters.Filters.Add(new InitialConfigurationFilter());
            if (Settings.EnableInitialConfiguration)
            {
                context.MapRoute(
                    "InitialConfiguration_default",
                    "InitialConfiguration",
                    new
                    {
                        controller = "Home",
                        action = "Index"
                    }
                );
            }
        }
    }
}
