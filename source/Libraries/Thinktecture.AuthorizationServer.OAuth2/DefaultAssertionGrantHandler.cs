using System.Collections.Generic;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class DefaultAssertionGrantHandler : IAssertionGrantHandler
    {
        public ClaimsIdentity ProcessAssertion(ValidatedRequest validatedRequest)
        {
            return null;
        }

        public IEnumerable<string> SupportedAssertions
        {
            get { return new string[] { }; }
        }
    }
}
