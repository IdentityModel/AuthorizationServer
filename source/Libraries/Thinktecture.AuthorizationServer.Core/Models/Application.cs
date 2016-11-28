/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens;
using System.Linq;

namespace Thinktecture.AuthorizationServer.Models
{
    public class Application
    {
        [Key]
        public virtual int ID { get; set; }

        [Required]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string LogoUrl { get; set; }

        // {appName}
        // unique, URL friendly chars
        [Required]
        public virtual string Namespace { get; set; }

        [Required]
        public virtual string Audience { get; set; }

        [Range(0, Int32.MaxValue)]
        public virtual int TokenLifetime { get; set; }
        public virtual bool AllowRefreshToken { get; set; }
        public virtual bool RequireConsent { get; set; }
        public virtual bool AllowRememberConsentDecision { get; set; }
        public virtual bool AllowSlidingRefreshTokenExpiration { get; set; }

        public virtual bool Enabled { get; set; }

        [Required]
        public virtual SigningKey SigningKey { get; set; }

        public virtual List<Scope> Scopes { get; set; }
        
        public IEnumerable<Client> Clients
        {
            get
            {
                var query =
                    from s in this.Scopes ?? Enumerable.Empty<Scope>()
                    select s.AllowedClients;
                var result = query.SelectMany(x => x);
                return result;
            }
        }

        public SigningCredentials SigningCredentials
        {
            get
            {
                if (this.SigningKey == null) return null;
                return this.SigningKey.GetSigningCredentials();
            }
        }
    }
}