using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.OAuth2;

namespace Thinktecture.Samples
{
    public class AssertionGrantHandler : IAssertionGrantHandler
    {
        public IEnumerable<string> SupportedAssertions
        {
            get { throw new NotImplementedException(); }
        }

        public ClaimsIdentity ProcessAssertion(ValidatedRequest validatedRequest)
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
                id.AddClaim(new Claim("sub", msaId));

                return id;
            }

            return null;
        }
    }
}
