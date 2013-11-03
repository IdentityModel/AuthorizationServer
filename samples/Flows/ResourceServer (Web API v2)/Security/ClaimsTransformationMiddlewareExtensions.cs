using System;
using System.Security.Claims;
using Thinktecture.IdentityModel.Owin;

namespace Owin
{
    public static class ClaimsTransformationMiddlewareExtensions
    {
        public static IAppBuilder UseClaimsTransformation(this IAppBuilder app, ClaimsAuthenticationManager manager)
        {
            return app.UseClaimsTransformation(new ClaimsTransformationOptions
                {
                    ClaimsAuthenticationManager = manager
                });
        }

        public static IAppBuilder UseClaimsTransformation(this IAppBuilder app, ClaimsTransformationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(ClaimsTransformationMiddleware), options);
            return app;
        }
    }
}