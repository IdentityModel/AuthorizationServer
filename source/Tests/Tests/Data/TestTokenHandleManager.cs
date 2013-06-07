using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Test
{
    class TestTokenHandleManager : ITokenHandleManager
    {
        string _clientId;
        string _redirectUri;

        public TestTokenHandleManager(string clientId, string redirectUri)
        {
            _clientId = clientId;
            _redirectUri = redirectUri;
        }

        public void Add(Models.TokenHandle handle)
        {
            
        }

        public Models.TokenHandle Get(string handleIdentifier)
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

        public void Delete(string handleIdentifier)
        {
            
        }
    }
}
