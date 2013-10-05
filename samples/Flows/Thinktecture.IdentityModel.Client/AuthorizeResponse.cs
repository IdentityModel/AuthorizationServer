/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System;
using System.Collections.Generic;

namespace Thinktecture.IdentityModel.Client
{
    public class AuthorizeResponse
    {
        public enum ResponseTypes
        {
            AuthorizationCode,
            Token,
            Error
        };

        public ResponseTypes ResponseType { get; protected set; }
        public string Raw { get; protected set; }
        public Dictionary<string, string> Values { get; protected set; }

        public string Code
        {
            get
            {
                return TryGet(OAuth2Constants.Code);
            }
        }

        public string AccessToken
        {
            get
            {
                return TryGet(OAuth2Constants.AccessToken);
            }
        }

        public string Error
        {
            get
            {
                return TryGet(OAuth2Constants.Error);
            }
        }

        public long ExpiresIn
        {
            get
            {
                var value = TryGet(OAuth2Constants.ExpiresIn);

                long longValue = 0;
                long.TryParse(value, out longValue);

                return longValue;
            }
        }

        public string Scope
        {
            get
            {
                return TryGet(OAuth2Constants.Scope);
            }
        }

        public string TokenType
        {
            get
            {
                return TryGet(OAuth2Constants.TokenType);
            }
        }

        public AuthorizeResponse(string raw)
        {
            Raw = raw;
            Values = new Dictionary<string, string>();
            ParseRaw();
        }

        private void ParseRaw()
        {
            var queryParameters = new Dictionary<string, string>();
            string[] fragments = null;

            if (Raw.Contains("#"))
            {
                fragments = Raw.Split('#');
                ResponseType = ResponseTypes.Token;
            }
            else if (Raw.Contains("?"))
            {
                fragments = Raw.Split('?');
                ResponseType = ResponseTypes.AuthorizationCode;
            }
            else
            {
                throw new InvalidOperationException("Malformed callback URL");
            }

            if (Raw.Contains(OAuth2Constants.Error))
            {
                ResponseType = ResponseTypes.Error;
            }

            var qparams = fragments[1].Split('&');

            foreach (var param in qparams)
            {
                var parts = param.Split('=');
                
                if (parts.Length == 2)
                {
                    Values.Add(parts[0], parts[1]);
                }
                else
                {
                    throw new InvalidOperationException("Malformed callback URL.");
                }
            }
        }

        private string TryGet(string type)
        {
            string value;
            if (Values.TryGetValue(type, out value))
            {
                return value;
            }

            return null;
        }
    }
}