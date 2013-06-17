/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class NameIdToSubjectClaimsTransformer : ClaimsTransformerBase
    {
        public NameIdToSubjectClaimsTransformer(IAuthorizationServerAdministratorsService svc)
            : base(svc)
        { }

        protected override Claim GetSubject(ClaimsPrincipal principal)
        {
            var nameId = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (nameId == null)
            {
                nameId = principal.FindFirst(ClaimTypes.Name);
                if (nameId == null)
                {
                    throw new InvalidOperationException("No nameidentifier claim");
                }
            }

            return new Claim(Constants.ClaimTypes.Subject, nameId.Value);
        }
    }
}