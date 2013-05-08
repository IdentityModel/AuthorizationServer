/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Thinktecture.AuthorizationServer.Models
{
    public class TokenHandle
    {
        public TokenHandle()
        {
            HandleId = Guid.NewGuid().ToString("N");
        }

        public string HandleId { get; set; }
        public string ClientId { get; set; }
        public ClaimsPrincipal ResourceOwner { get; set; }
        public List<Scope> Scopes { get; set; }
        public TokenHandleType Type { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
