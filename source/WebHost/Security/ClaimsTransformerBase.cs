﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public abstract class ClaimsTransformerBase : ClaimsAuthenticationManager
    {
        protected IAuthorizationServerAdministratorsService service;
        protected abstract string GetSubject(ClaimsPrincipal principal);
       
        public ClaimsTransformerBase(IAuthorizationServerAdministratorsService svc)
        {
            this.service = svc;
        }

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            var subject = GetSubject(incomingPrincipal);
            var claims = new List<Claim> { new Claim(Constants.ClaimTypes.Subject, subject) };

            claims.AddRange(AddInternalClaims(subject));
            claims.AddRange(AddExternalClaims(incomingPrincipal));


            return Principal.Create("AuthorizationServer", claims.ToArray());
        }

        protected virtual IEnumerable<Claim> AddInternalClaims(string subject)
        {
            var adminNameIDs = this.service.GetAdministratorNameIDs();
            var result = new List<Claim>();

            if (adminNameIDs.Any(a => a.Equals(subject, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                result.Add(new Claim(
                    ClaimTypes.Role, 
                    Constants.Roles.Administrators, 
                    ClaimValueTypes.String, 
                    Constants.InternalIssuer));
            }

            return result;
        }

        private IEnumerable<Claim> AddExternalClaims(ClaimsPrincipal incomingPrincipal)
        {
            var claims = new List<Claim>();

            var filterClaims = ConfigurationManager.AppSettings["authz:FilterIncomingClaims"];
            if (!string.IsNullOrEmpty(filterClaims))
            {
                bool filterFlag;
                if (bool.TryParse(filterClaims, out filterFlag))
                {
                    if (!filterFlag)
                    {
                        claims.AddRange(from c in incomingPrincipal.Claims
                                        where !c.Type.Equals(Constants.ClaimTypes.Subject)
                                        select c);
                    }
                }
            }

            return claims;
        }
    }
}