using System;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Test
{
    class TestClientManager : IClientManager
    {
        string _id;
        string _secret;

        public TestClientManager(string id, string secret)
        {
            _id = id;
            _secret = secret;
        }

        public Models.Client Get(string id)
        {
            return new Client() { 
                ClientId = _id, 
                ClientSecret = _secret 
            };
        }
    }
}
