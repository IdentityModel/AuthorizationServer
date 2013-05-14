using System.Collections.Generic;
using System.Linq;

namespace Thinktecture.AuthorizationServer.Models
{
    public static class RedirectUriExtensions
    {
        public static RedirectUri Get(this IEnumerable<RedirectUri> uris, string uri)
        {
            return (from u in uris
                    where u.Uri.Equals(uri)
                    select u)
                   .SingleOrDefault();
        }
    }
}
