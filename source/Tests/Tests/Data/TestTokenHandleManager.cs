using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Test
{
    class TestTokenHandleManager : ITokenHandleManager
    {
        string _clientId;
        string _redirectUri;
        string _id;

        public TestTokenHandleManager(string id, string clientId, string redirectUri)
        {
            _clientId = clientId;
            _redirectUri = redirectUri;
            _id = id;
        }

        public void Add(Models.TokenHandle handle)
        {
            
        }

        public Models.TokenHandle Get(string handleIdentifier)
        {
            if (handleIdentifier == _id)
            {
                var handle = new TokenHandle
                {
                    Client = new Client
                    {
                        ClientId = _clientId
                    },

                    RedirectUri = _redirectUri
                };

                return handle;
            }

            return null;
        }

        public void Delete(string handleIdentifier)
        {
            
        }


        public TokenHandle Find(string subject, Client client, Application application)
        {
            throw new System.NotImplementedException();
        }
    }
}
