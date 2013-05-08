using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public interface ITokenHandleManager
    {
        string Add(TokenHandle handle);
        bool TryGet(string handleIdentifier, out TokenHandle handle);
        void Delete(string handleIdentifier);
        
        //IEnumerable<CodeToken> Search(int? clientId, string username, string scope, CodeTokenType type)
    }
}
