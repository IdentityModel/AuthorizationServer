/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string ResponseString { get; protected set; }

        public string Code { get; set; }
        public string AccessToken { get; set; }
        public string Error { get; set; }
        public long ExpiresIn { get; set; }
        public string Scopes { get; set; }
        public string TokenType { get; set; }

        public AuthorizeResponse(string responseString)
        {
            ResponseString = responseString;
            ParseResponse();
        }

        private void ParseResponse()
        {
            if (ResponseString.Contains("error"))
            {
                ParseError();
            }
            else if (ResponseString.Contains("#"))
            {
                ParseImplicitResponse();
            }
            else
            {
                ParseCodeResponse();
            }
        }

        private void ParseError()
        {
            ResponseType = ResponseTypes.Error;

            throw new NotImplementedException();
        }

        private void ParseCodeResponse()
        {
            ResponseType = ResponseTypes.AuthorizationCode;


            throw new NotImplementedException();
        }

        private void ParseImplicitResponse()
        {
            ResponseType = ResponseTypes.Token;

            var fragments = ResponseString.Split('#');
            var qparams = fragments[1].Split('&');

            foreach (var param in qparams)
            {
                var parts = param.Split('=');
                if (parts.Length == 2)
                {
                    if (parts[0].Equals("access_token", StringComparison.Ordinal))
                    {
                        AccessToken = parts[1];

                    }
                    else if (parts[0].Equals("expires_in", StringComparison.Ordinal))
                    {
                        ExpiresIn = long.Parse(parts[1]);
                    }
                    else if (parts[0].Equals("token_type", StringComparison.Ordinal))
                    {
                        TokenType = parts[1];
                    }
                }
                else
                {
                    throw new InvalidOperationException("Malformed token response.");
                }
            }
        }
    }
}