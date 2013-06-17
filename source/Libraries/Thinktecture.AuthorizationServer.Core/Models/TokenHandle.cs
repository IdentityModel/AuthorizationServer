/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace Thinktecture.AuthorizationServer.Models
{
    public class TokenHandle
    {
        [Key]
        public virtual string HandleId { get; set; }

        public string Subject { get; set; }
        public virtual Client Client { get; set; }
        public virtual Application Application { get; set; }

        public virtual string RedirectUri { get; set; }
        public virtual TokenHandleType Type { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime? Expiration { get; set; }

        public virtual bool CreateRefreshToken { get; set; }
        public virtual DateTime? RefreshTokenExpiration { get; set; }

        public virtual List<TokenHandleClaim> ResourceOwner { get; set; }
        public virtual List<Scope> Scopes { get; set; }

        public static TokenHandle CreateRefreshTokenHandle(string subject, Client client, Application application, IEnumerable<Claim> claims, IEnumerable<Scope> scopes, DateTime? expiration = null)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (application == null) throw new ArgumentNullException("application");
            if (claims == null) throw new ArgumentNullException("claims");
            if (scopes == null) throw new ArgumentNullException("scopes");

            return new TokenHandle
            {
                Type = TokenHandleType.RefreshTokenIdentifier,
                Subject = subject,
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
        public virtual int ID { get; set; }
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }
    }
}
