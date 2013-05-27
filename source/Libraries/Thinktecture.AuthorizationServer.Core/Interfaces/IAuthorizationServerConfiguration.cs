/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface IAuthorizationServerConfiguration
    {
        GlobalConfiguration GlobalConfiguration { get; }
        Application FindApplication(string url);
    }
}
