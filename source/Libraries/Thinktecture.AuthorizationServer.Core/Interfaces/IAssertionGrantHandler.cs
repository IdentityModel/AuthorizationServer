using System.Collections.Generic;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface IAssertionGrantHandler
    {
        IEnumerable<string> SupportedAssertions { get; }
        ClaimsIdentity ProcessAssertion(ValidatedRequest validatedRequest);
    }
}
