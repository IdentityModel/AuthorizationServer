/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class SubjectClaimsTransformer : ClaimsTransformerBase
    {
        public SubjectClaimsTransformer(IAuthorizationServerAdministratorsService svc)
            : base(svc)
        { }

        protected override string GetSubject(ClaimsPrincipal principal)
        {
            var subject = principal.FindFirst(Constants.ClaimTypes.Subject);
            if (subject == null)
            {
                subject = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (subject == null)
                {
                    subject = principal.FindFirst(ClaimTypes.Name);
                    if (subject == null)
                    {
                        throw new InvalidOperationException("No subject identifier claim");
                    }
                }
            }

            return subject.Value;
        }
    }
}