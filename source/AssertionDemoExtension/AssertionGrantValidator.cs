using Microsoft.Live;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.OAuth2;

namespace Thinktecture.Samples
{
    public class AssertionGrantValidator : IAssertionGrantValidation
    {
        public IEnumerable<string> SupportedAssertionTypes
        {
            get 
            { 
                return new[] { OAuthConstants.GrantTypes.MsaIdentityToken }; 
            }
        }

        public ClaimsPrincipal ValidateAssertion(ValidatedRequest validatedRequest)
        {
            if (validatedRequest.AssertionType == OAuthConstants.GrantTypes.MsaIdentityToken)
            {
                var appId = "ms-app://s-1-15-2-566730974-2602954100-374302646-1642987517-3006637339-1063681522-1721721910";
                var appSecret = "jqRW49YVlEy3qzKuNdnBQy2JYB4KxOxv";
                var redirectUri = "http://www.thinktecture.com";

                var authClient = new LiveAuthClient(
                       appId,
                       appSecret,
                       redirectUri);

                var msaId = authClient.GetUserId(validatedRequest.Assertion);
                var id = new ClaimsIdentity("MSA");
                id.AddClaim(new Claim(ClaimTypes.NameIdentifier, msaId));

                return FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager.Authenticate(
                    "AssertionValidation", 
                    new ClaimsPrincipal(id));
            }

            return null;
        }
    }
}