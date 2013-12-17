using System.Security.Claims;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface IAssertionGrantValidation
    {
        ClaimsPrincipal ValidateAssertion(ValidatedRequest validatedRequest);
    }
}