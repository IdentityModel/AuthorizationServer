using System;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Test
{
    class TestTokenHandleManager : ITokenHandleManager
    {
        string _clientId;
        string _redirectUri;
        string _id;
        bool _expired;

        public TestTokenHandleManager(string id, string clientId, string redirectUri, bool expired = false)
        {
            _clientId = clientId;
            _redirectUri = redirectUri;
            _id = id;
            _expired = expired;
        }

        public void Add(Models.TokenHandle handle)
        {
            
        }

        public Models.TokenHandle Get(string handleIdentifier)
        {
            DateTime expiration;
            if (_expired)
            {
                expiration = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));
            }
            else
            {
                expiration = DateTime.UtcNow.Add(TimeSpan.FromHours(1));
            }

            if (handleIdentifier == _id)
            {
                var handle = new TokenHandle
                {
                    Client = new Client
                    {
                        ClientId = _clientId
                    },

                    RedirectUri = _redirectUri,
                    Expiration = expiration
                };

                return handle;
            }

            return null;
        }

        public void Delete(string handleIdentifier)
        {
            
        }

        public TokenHandle Find(string subject, Client client, Application application, System.Collections.Generic.IEnumerable<Scope> scopes, TokenHandleType type)
        {
            throw new System.NotImplementedException();
        }
    }
}
