using System.Collections.Generic;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface IAssertionGrantValidation
    {
        IEnumerable<string> SupportedAssertionTypes { get; }
        ClaimsPrincipal ValidateAssertion(ValidatedRequest validatedRequest);
    }
}