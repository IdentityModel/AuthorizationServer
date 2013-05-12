/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Web.Mvc;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    [Serializable]
    public class AuthorizeRequestValidationException : Exception
    {
        public AuthorizeRequestValidationException(string message) : base(message)
        { }
    }
}
