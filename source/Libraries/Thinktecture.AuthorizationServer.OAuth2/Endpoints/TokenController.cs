/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.AuthorizationServer.OAuth2;

namespace Authorization_Prototype.Controllers.OAuth2
{
    public class TokenController : ApiController
    {
        public HttpResponseMessage Get(string appName, TokenRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
