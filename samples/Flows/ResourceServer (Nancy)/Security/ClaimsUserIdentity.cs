using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ResourceServer.Security
{
    public class ClaimsUserIdentity : IUserIdentity
    {
        ClaimsPrincipal _principal;

        public ClaimsUserIdentity(ClaimsPrincipal principal)
        {
            _principal = principal;
        }

        public ClaimsPrincipal Principal
        {
            get
            {
                return _principal;
            }
        }

        public IEnumerable<string> Claims
        {
            get { throw new NotImplementedException(); }
        }

        public string UserName
        {
            get { return "ClaimsPrincipal"; }
        }
    }
}