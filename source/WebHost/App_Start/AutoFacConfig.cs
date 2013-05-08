using Autofac;
using Autofac.Configuration;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System.Web.Http;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.OAuth2;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public static class AutofacConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<WSTrustResourceOwnerCredentialValidation>().
            //    As<IResourceOwnerCredentialValidation>();
            
            builder.RegisterType<DummyTokenHandleManager>().
                As<ITokenHandleManager>();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));

            builder.RegisterControllers(typeof(AutofacConfig).Assembly); 
            var container = builder.Build(); 
            
            // MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // Web API
            var resolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }
    }
}