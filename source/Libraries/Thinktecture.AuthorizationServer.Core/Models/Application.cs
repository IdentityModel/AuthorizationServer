/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens;

namespace Thinktecture.AuthorizationServer.Models
{
    public class Application
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }

        // {appName}
        // unique, URL friendly chars
        public string Namespace { get; set; }
        
        public string Audience { get; set; }

        public int TokenLifetime { get; set; }
        public bool AllowRefreshToken { get; set; }
        public bool RequireConsent { get; set; }
        public bool RememberConsentDecision { get; set; }
        
        public SigningKey SigningKey { get; set; }

        public List<Scope> Scopes { get; set; }
        
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