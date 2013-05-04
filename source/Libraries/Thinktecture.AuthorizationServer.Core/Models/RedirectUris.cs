using System.Collections.Generic;
using System.Linq;

namespace Thinktecture.AuthorizationServer.Core.Models
{
    public class RedirectUris : List<RedirectUri>
    {
        public RedirectUri Get(string uri)
        {
            return (from u in this
                    where u.Uri.Equals(uri)
                    select u)
                   .SingleOrDefault();
        }
    }
}
