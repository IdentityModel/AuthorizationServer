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
    public class StoredGrant
    {
        [Key]
        public virtual string GrantId { get; set; }

        public string Subject { get; set; }
        public virtual Client Client { get; set; }
        public virtual Application Application { get; set; }
        public virtual List<StoredGrantClaim> ResourceOwner { get; set; }
        public virtual List<Scope> Scopes { get; set; }
        public virtual string RedirectUri { get; set; }
        public virtual StoredGrantType Type { get; set; }
        
        public virtual DateTime Created { get; set; }
        public virtual DateTime Expiration { get; set; }

        public virtual bool CreateRefreshToken { get; set; }

public virtual DateTime? RefreshTokenExpiration { get;set; }

        public static StoredGrant CreateConsentDecision(string subject, Client client, Application application, IEnumerable<Scope> scopes)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (application == null) throw new ArgumentNullException("application");
            if (scopes == null) throw new ArgumentNullException("scopes");

            return new StoredGrant
            {
                Type = StoredGrantType.ConsentDecision,
                Subject = subject,
                Client = client,
                Application = application, 
                Scopes = scopes.ToList(),
                Created = DateTime.UtcNow,
                Expiration = DateTime.UtcNow.AddYears(5)
            };
        }

        public static StoredGrant CreateRefreshTokenHandle(string subject, Client client, Application application, IEnumerable<Claim> claims, IEnumerable<Scope> scopes, DateTime? expiration)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (application == null) throw new ArgumentNullException("application");
            if (claims == null) throw new ArgumentNullException("claims");
            if (scopes == null) throw new ArgumentNullException("scopes");

            return new StoredGrant
            {
                Type = StoredGrantType.RefreshTokenIdentifier,
                Subject = subject,
                Client = client,
                Application = application,
                ResourceOwner = claims.ToStoredGrantClaims().ToList(),
                Scopes = scopes.ToList(),
                Created = DateTime.UtcNow,
                Expiration = expiration.Value != System.DateTime.MinValue ? expiration.Value : DateTime.UtcNow.AddHours(1)
            };
        }

        public static StoredGrant CreateAuthorizationCode(
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

            return new StoredGrant
            {
                Type = StoredGrantType.AuthorizationCode,
                Client = client,
                Application = application,
                RedirectUri = redirectUri,
                ResourceOwner = claims.ToStoredGrantClaims().ToList(),
                Scopes = scopes.ToList(),
                Created = DateTime.UtcNow,
                Expiration = expiration?? DateTime.UtcNow.AddHours(1),
                CreateRefreshToken = createRefreshToken,
                RefreshTokenExpiration = (!createRefreshToken ? null : (refreshTokenExpiration != System.DateTime.MinValue ? refreshTokenExpiration : DateTime.UtcNow.AddHours(1)))
            };
        }


        public StoredGrant()
        {
            GrantId = Guid.NewGuid().ToString("N");
        }
    }

    public class StoredGrantClaim
    {
        public virtual int ID { get; set; }
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }
    }
}
