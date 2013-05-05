/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using System.Linq;

namespace Thinktecture.AuthorizationServer.Core.Models
{
    public class Scopes : List<Scope>
    {
        public List<Scope> GetScopesForClient(string clientId)
        {
            var result = from s in this
                         where s.AllowedClients.Select(c => c.ClientId).Contains(clientId)
                         select s;

            return result.ToList();
        }

        public List<string> GetScopeNamesForClient(string clientId)
        {
            return GetScopesForClient(clientId).Select(s => s.Name).ToList();
        }

        public bool TryValidateScopes(string clientId, List<string> requestedScopes, out List<Scope> resultingScopes)
        {
            var allowedScopeNames = GetScopeNamesForClient(clientId);

            resultingScopes = new List<Scope>();
            foreach (var scope in requestedScopes)
            {
                if (allowedScopeNames.Contains(scope))
                {
                    var allowedScope = from asc in this
                                       where asc.Name.Equals(scope)
                                       select asc;

                    resultingScopes.Add(allowedScope.Single());
                }
                else
                {
                    Tracing.Error("Scope not allowed: " + scope);
                    return false;
                }


            }

            return true;
        }
    }
}