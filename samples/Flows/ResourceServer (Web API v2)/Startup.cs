using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Thinktecture.Samples.Startup))]
namespace Thinktecture.Samples
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AuthConfig.Configure(app);
            WebApiConfig.Configure(app);
        }
    }
}