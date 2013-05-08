/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Web.Mvc;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    [Serializable]
    public class AuthorizeRequestClientException : AuthorizeRequestValidationException
    {
        public AuthorizeRequestClientException(string message, Uri redirectUri, string error, string responseType, string state = null)
        {
            Tracing.Error(message);
            Result = new ClientErrorResult(redirectUri, error, responseType, state);
        }
    }
}
