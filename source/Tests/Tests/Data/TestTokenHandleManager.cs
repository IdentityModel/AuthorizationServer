using System;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Test
{
    class TestTokenHandleManager : IStoredGrantManager
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

        public void Add(Models.StoredGrant handle)
        {
            
        }

        public Models.StoredGrant Get(string handleIdentifier)
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
                var handle = new StoredGrant
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

        public StoredGrant Find(string subject, Client client, Application application, System.Collections.Generic.IEnumerable<Scope> scopes, StoredGrantType type)
        {
            throw new System.NotImplementedException();
        }
    }
}
