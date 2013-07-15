/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface IStoredGrantManager
    {
        void Add(StoredGrant grant);
        StoredGrant Get(string grantIdentifier);
        StoredGrant Find(string subject, Client client, Application application, IEnumerable<Scope> scopes, StoredGrantType type);
        void Delete(string grantIdentifier);
    }
}
