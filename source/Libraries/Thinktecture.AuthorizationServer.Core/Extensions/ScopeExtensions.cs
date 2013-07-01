using System.Collections.Generic;
using System.Linq;

namespace Thinktecture.AuthorizationServer.Models
{
    public static class ScopeExtensions
    {
        public static List<Scope> GetScopesForClient(this IEnumerable<Scope> scopes, string clientId)
        {
            var result = from s in scopes
                         where s.AllowedClients.Select(c => c.ClientId).Contains(clientId)
                         select s;

            return result.ToList();
        }

        public static List<string> GetScopeNamesForClient(this IEnumerable<Scope> scopes, string clientId)
        {
            return scopes.GetScopesForClient(clientId).Select(s => s.Name).ToList();
        }

        public static bool TryValidateScopes(this IEnumerable<Scope> scopes, string clientId, List<string> requestedScopes, out List<Scope> resultingScopes)
        {
            var allowedScopeNames = scopes.GetScopeNamesForClient(clientId);

            resultingScopes = new List<Scope>();
            foreach (var scope in requestedScopes)
            {
                if (allowedScopeNames.Contains(scope))
                {
                    var allowedScope = from asc in scopes
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

        public static bool ScopeEquals(this IEnumerable<Scope> requestedScopes, IEnumerable<Scope> storedScopes)
        {
            var storedScopeNames = storedScopes.OrderBy(s => s.Name).Select(s => s.Name).ToArray();
            var requestedScopeNames = requestedScopes.OrderBy(s => s.Name).Select(s => s.Name).ToArray();

            return storedScopeNames.SequenceEqual(requestedScopeNames);
        }
    }
}
