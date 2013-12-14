/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public static class OAuthConstants
    {
        public static class GrantTypes
        {
            public const string Password = "password";
            public const string AuthorizationCode = "authorization_code";
            public const string ClientCredentials = "client_credentials";
            public const string RefreshToken = "refresh_token";
            public const string Assertion = "assertion";
            
            // assertion grants
            public const string Saml2 = "urn:ietf:params:oauth:grant-type:saml2-bearer";
            public const string JwtBearer = "urn:ietf:params:oauth:grant-type:jwt-bearer";
            public const string MsaIdentityToken = "urn:msaidentitytoken";
        }

        public static class ResponseTypes
        {
            public const string Token = "token";
            public const string Code = "code";
        }

        public static class Errors
        {
            public const string Error = "error";
            public const string InvalidRequest = "invalid_request";
            public const string InvalidClient = "invalid_client";
            public const string InvalidGrant = "invalid_grant";
            public const string UnauthorizedClient = "unauthorized_client";
            public const string UnsupportedGrantType = "unsupported_grant_type";
            public const string UnsupportedResponseType = "unsupported_response_type";
            public const string InvalidScope = "invalid_scope";
            public const string AccessDenied = "access_denied";
            public const string ServerError = "server_error";
        }
    }
}