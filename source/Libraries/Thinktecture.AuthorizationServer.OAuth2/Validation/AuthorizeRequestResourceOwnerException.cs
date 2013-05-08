/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Web.Mvc;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    [Serializable]
    public class AuthorizeRequestResourceOwnerException : AuthorizeRequestValidationException
    {
        public AuthorizeRequestResourceOwnerException(string message)
        {
            Tracing.Error(message);
            Result = ResourceOwnerError(message);
        }

        private ActionResult ResourceOwnerError(string message)
        {
            var result = new ViewResult
            {
                ViewName = "ValidationError",
            };

            result.ViewBag.Message = message;

            return result;
        }
    }
}
   
