using Owin;
using System.Web.Http;

namespace Thinktecture.Samples
{
    public class WebApiConfig
    {
        public static void Configure(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            config.EnableCors();

            app.UseWebApi(config);
        }
    }
}