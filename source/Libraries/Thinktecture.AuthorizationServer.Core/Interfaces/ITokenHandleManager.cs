/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface ITokenHandleManager
    {
        void Add(TokenHandle handle);
        bool TryGet(string handleIdentifier, out TokenHandle handle);
        void Delete(string handleIdentifier);
        
        //IEnumerable<CodeToken> Search(int? clientId, string username, string scope, CodeTokenType type)
    }
}
