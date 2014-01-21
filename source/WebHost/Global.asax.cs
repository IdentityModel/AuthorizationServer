/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.IdentityModel.Services;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.IdentityModel.Tokens;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            System.IdentityModel.Tokens.JwtSecurityTokenHandler.OutboundClaimTypeMap = ClaimMappings.None;

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            AutofacConfig.Configure();
            DataProtectionConfig.Configure();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            FederatedAuthentication.FederationConfigurationCreated += FederatedAuthentication_FederationConfigurationCreated;
        }

        void FederatedAuthentication_FederationConfigurationCreated(object sender, System.IdentityModel.Services.Configuration.FederationConfigurationCreatedEventArgs e)
        {
            var svc = DependencyResolver.Current.GetService<IAuthorizationServerAdministratorsService>();

            e.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager = new NameIdToSubjectClaimsTransformer(svc);
            e.FederationConfiguration.IdentityConfiguration.ClaimsAuthorizationManager = new AuthorizationManager();
        }
    }
}