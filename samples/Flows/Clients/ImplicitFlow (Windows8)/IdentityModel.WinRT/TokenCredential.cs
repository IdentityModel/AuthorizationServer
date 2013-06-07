using System;

namespace Thinktecture.IdentityModel.WinRT
{
    public class TokenCredential
    {
        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public DateTime Expires { get; set; }
    }
}
