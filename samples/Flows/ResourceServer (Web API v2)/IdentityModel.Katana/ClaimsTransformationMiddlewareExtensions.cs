using System.Security.Claims;
using Thinktecture.IdentityModel.Owin;

namespace Owin
{
    public static class ClaimsTransformationMiddlewareExtensions
    {
        public static IAppBuilder UseClaimsTransformation(this IAppBuilder app, ClaimsAuthenticationManager claimsAuthenticationManager)
        {
            app.Use(typeof(ClaimsTransformationMiddleware), claimsAuthenticationManager);
            return app;
        }
    }
}