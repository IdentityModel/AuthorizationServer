/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Security.Claims;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface IResourceOwnerCredentialValidation
    {
        ClaimsPrincipal Validate(string userName, string password);
    }
}
