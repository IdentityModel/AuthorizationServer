using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface IAuthorizationServerConfiguration
    {
        Application FindApplication(string url);
    }
}
