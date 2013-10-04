/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using Newtonsoft.Json.Linq;

namespace Thinktecture.IdentityModel.Client
{
    public class TokenResponse
    {
        public string Raw { get; protected set; }
        public JObject Json { get; protected set; }

        public TokenResponse(string raw)
        {
            Raw = raw;
            Json = JObject.Parse(raw);
        }

        public string AccessToken
        {
            get
            {
                return GetStringOrNull(OAuth2Constants.AccessToken);
            }
        }

        public long ExpiresIn
        {
            get
            {
                return GetLongOrNull(OAuth2Constants.ExpiresIn);
            }
        }

        public string TokenType
        {
            get
            {
                return GetStringOrNull(OAuth2Constants.TokenType);
            }
        }

        public string RefreshToken
        {
            get
            {
                return GetStringOrNull(OAuth2Constants.RefreshToken);
            }
        }

        protected virtual string GetStringOrNull(string name)
        {
            var value = Json[name];

            if (name != null)
            {
                return value.ToString();
            }

            return null;
        }

        protected virtual long GetLongOrNull(string name)
        {
            var value = Json[name];

            if (name != null)
            {
                long longValue = 0;
                if (long.TryParse(value.ToString(), out longValue))
                {
                    return longValue;
                }
            }

            return 0;
        }
    }
}
