/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Linq;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface IRepository<T>
    {
        IQueryable<T> All { get; }
        void Add(T item);
        void Remove(T item);
    }

    public interface IAuthorizationServerAdministration
    {
        GlobalConfiguration GlobalConfiguration { get; set; }
        IRepository<Application> Applications { get; }
        IRepository<Scope> Scopes { get; }
        IRepository<Client> Clients { get; }
        IRepository<ClientRedirectUri> ClientRedirects { get; }
        IRepository<SigningKey> Keys { get; }
        IRepository<StoredGrant> Tokens { get; }

        void SaveChanges();
    }
}
