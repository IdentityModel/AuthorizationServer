using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinktecture.Samples;

[assembly: OwinStartup(typeof(ResourceServer.Startup))]

namespace ResourceServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // validate JWT tokens from AuthorizationServer
            app.UseJsonWebToken(
                issuer: Constants.AS.IssuerName,
                audience: Constants.Audience,
                signingKey: Constants.AS.SigningKey);

            app.UseNancy();
        }
    }
}