using System.Security.Claims;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public interface IResourceOwnerCredentialValidation
    {
        ClaimsPrincipal Validate(string userName, string password);
    }
}
