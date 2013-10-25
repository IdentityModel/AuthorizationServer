using Microsoft.Owin;
using Microsoft.Owin.Security;
using Nancy;
using Nancy.Owin;
using System.Collections.Generic;
using System.Security.Claims;

public static class NancyOwinContextExtensions
{
    /// <summary>
    ///     Gets the OWIN authentication manager from the nancy context.
    /// </summary>
    /// <param name="context">The current nancy context.</param>
    /// <returns>An <see cref="IAuthenticationManager" />.</returns>
    public static IAuthenticationManager GetOwinAuthentication(this NancyContext context)
    {
        var environment = (IDictionary<string, object>)context.Items[NancyOwinHost.RequestEnvironmentKey];
        var owinContext = new OwinContext(environment);
        return owinContext.Authentication;
    }

    public static ClaimsPrincipal GetOwinPrincipal(this NancyContext context)
    {
        var auth = context.GetOwinAuthentication();
        return auth.User;
    }
}