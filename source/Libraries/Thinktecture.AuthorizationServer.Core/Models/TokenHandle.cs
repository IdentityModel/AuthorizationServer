/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public List<Claim> ResourceOwner { get; set; }
        public List<string> Scopes { get; set; }
        public TokenHandleType Type { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool DoesExpire { get; set; }
        public DateTime Expiration { get; set; }
    }
}
