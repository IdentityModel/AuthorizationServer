/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Newtonsoft.Json;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class TokenRequest
    {
        public string Grant_Type { get; set; }
        public string Scope { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
        public string Refresh_Token { get; set; }
        public string Redirect_Uri { get; set; }
        public string Assertion { get; set; }
    }
}