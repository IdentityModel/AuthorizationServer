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
        [Key]
        public string HandleId { get; set; }
        
        public Client Client { get; set; }
        public Application Application { get; set; }

        public string RedirectUri { get; set; }
        public TokenHandleType Type { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Expiration { get; set; }
        
        public bool CreateRefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }

        public List<TokenHandleClaim> ResourceOwner { get; set; }
        public List<Scope> Scopes { get; set; }

        public static TokenHandle CreateRefreshTokenHandle(Client client, Application application, IEnumerable<Claim> claims, IEnumerable<Scope> scopes, DateTime? expiration = null)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (application == null) throw new ArgumentNullException("application");
            if (claims == null) throw new ArgumentNullException("claims");
            if (scopes == null) throw new ArgumentNullException("scopes");

            return new TokenHandle
            {
                Type = TokenHandleType.RefreshTokenIdentifier,
                Client = client,
                Application = application,
                ResourceOwner = claims.ToTokenHandleClaims().ToList(),
                Scopes = scopes.ToList(),
                Created = DateTime.UtcNow,
                Expiration = expiration
            };
        }

        public static TokenHandle CreateAuthorizationCode(
            Client client, 
            Application application, 
            string redirectUri, 
            IEnumerable<Claim> claims, 
            IEnumerable<Scope> scopes, 
            bool createRefreshToken,
            DateTime? refreshTokenExpiration = null,
            DateTime? expiration = null)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (application == null) throw new ArgumentNullException("application");
            if (claims == null) throw new ArgumentNullException("claims");
            if (scopes == null) throw new ArgumentNullException("scopes");

            return new TokenHandle
            {
                Type = TokenHandleType.AuthorizationCode,
                Client = client,
                Application = application,
                RedirectUri = redirectUri,
                ResourceOwner = claims.ToTokenHandleClaims().ToList(),
                Scopes = scopes.ToList(),
                Created = DateTime.UtcNow,
                Expiration = expiration,
                CreateRefreshToken = createRefreshToken,
                RefreshTokenExpiration = refreshTokenExpiration
            };
        }


        public TokenHandle()
        {
            HandleId = Guid.NewGuid().ToString("N");
        }
    }

    public class TokenHandleClaim
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
