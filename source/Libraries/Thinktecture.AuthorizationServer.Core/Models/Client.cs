/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.AuthorizationServer.Models
{
    public class Client
    {
        [Key]
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ClientAuthenticationMethod AuthenticationMethod { get; set; }
        public string Name { get; set; }
        public OAuthFlow Flow { get; set; }
        public bool AllowRefreshToken { get; set; }
        public bool RequireConsent { get; set; }

        public List<RedirectUri> RedirectUris { get; set; }
    }
}
