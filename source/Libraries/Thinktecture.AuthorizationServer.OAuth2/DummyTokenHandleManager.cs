/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class DummyTokenHandleManager : ITokenHandleManager
    {
        public void Add(TokenHandle handle)
        {
            
        }

        public bool TryGet(string handleIdentifier, out TokenHandle handle)
        {
            handle = new TokenHandle
            {
                HandleId = "123"
            };

            return true;
        }

        public void Delete(string handleIdentifier)
        {
            
        }
    }
}
