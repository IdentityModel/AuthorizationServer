using System;
using System.IdentityModel.Services;
using System.Security.Claims;
using Thinktecture.IdentityModel;

namespace Authorization_Prototype.Security
{
    public static class Authentication
    {
        public static ClaimsPrincipal SetAuthenticationSession(string subject, string identityProvider, string authenticationMethod, int ttl, bool isPersistent)
        {
            var principal = Principal.Create("web",
                new Claim("sub", subject),
                new Claim("idp", identityProvider),
                new Claim("met", authenticationMethod));

            // claims transformation ?

            SetAuthenticationSession(principal, ttl, isPersistent);
            return principal;
        }

        public static void SetAuthenticationSession(ClaimsPrincipal principal, int ttl, bool isPersistent)
        {
            var sam = FederatedAuthentication.SessionAuthenticationModule;

            var sessionToken = sam.CreateSessionSecurityToken(
                principal,
                "authzserver",
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(ttl),
                isPersistent);

            sam.WriteSessionTokenToCookie(sessionToken);
        }
    }
}