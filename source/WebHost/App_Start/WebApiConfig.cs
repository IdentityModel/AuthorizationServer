/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Web.Http;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "OAuth2 Token Endpoint",
                routeTemplate: "oauth/{appName}/token",
                defaults: new { Controller = "Token" }
            );
        }
    }
}
