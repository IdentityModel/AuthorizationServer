/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;

namespace Thinktecture.AuthorizationServer.Models
{
    public class ValidatedRequest
    {
        public Application Application { get; set; }
        public Client Client { get; set; }
        public ClientRedirectUri RedirectUri { get; set; }
        public string ResponseType { get; set; }
        public List<Scope> Scopes { get; set; }
        public string State { get; set; }
        public string GrantType { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string AuthorizationCode { get; set; }
        public string RefreshToken { get; set; }
        public TokenHandle TokenHandle { get; set; }
        public bool RequestingRefreshToken { get; set; }
        public bool ShowConsent { get; set; }
        public bool ShowRememberConsent { get; set; }
    }
}
