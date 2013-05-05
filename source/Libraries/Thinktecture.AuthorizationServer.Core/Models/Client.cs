/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

namespace Thinktecture.AuthorizationServer.Core.Models
{
    public class Client
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ClientSecretType { get; set; }
        public string Name { get; set; }
        public OAuthFlows Flow { get; set; }
        public bool AllowRefreshToken { get; set; }

        public RedirectUris RedirectUris { get; set; }
    }
}
