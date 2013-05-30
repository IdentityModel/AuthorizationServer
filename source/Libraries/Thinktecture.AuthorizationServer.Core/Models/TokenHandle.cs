/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Linq;
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

        public TokenHandle(
            string clientID,
            string redirectUrl, 
            TokenHandleType type, 
            IEnumerable<Claim> claims, 
            IEnumerable<Scope> scopes,
            DateTime? expiration = null)
            : this()
        {
            if (String.IsNullOrWhiteSpace(clientID)) throw new ArgumentNullException("clientID");
            if (clientID == null) throw new ArgumentNullException("claims");
            if (scopes == null) throw new ArgumentNullException("scopes");

            this.ClientId = clientID;
            this.RedirectUrl = redirectUrl;
            this.Type = type;
            this.Created = DateTime.UtcNow;
            this.Expiration = expiration;
            this.ResourceOwner = claims.ToTokenHandleClaims().ToList();
            this.Scopes = scopes.ToList();
        }

        [Key]
        public string HandleId { get; set; }
        public string ClientId { get; set; }
        public string RedirectUrl { get; set; }
        public TokenHandleType Type { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Expiration { get; set; }
        
        public List<TokenHandleClaim> ResourceOwner { get; set; }
        public List<Scope> Scopes { get; set; }
    }

    public class TokenHandleClaim
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
