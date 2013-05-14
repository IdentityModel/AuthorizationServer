/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using System.IdentityModel.Tokens;

namespace Thinktecture.AuthorizationServer.Models
{
    public class Application
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }

        public string Namespace { get; set; }
        public string Entropy { get; set; }

        public string IssuerName { get; set; }
        public string Audience { get; set; }
        public string TokenType { get; set; }
        public int TokenLifetime { get; set; }
        public bool RequireConsent { get; set; }
        public bool RememberConsentDecision { get; set; }

        public List<Scope> Scopes { get; set; }
        public List<Client> Clients { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
    }
}