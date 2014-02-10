/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "OAuth2 Token Endpoint",
                routeTemplate: "{appName}/oauth/token",
                defaults: new { Controller = "Token" }
            );

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //config.EnableSystemDiagnosticsTracing();
        }
    }
}
