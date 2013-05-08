/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using System.Linq;

namespace Thinktecture.AuthorizationServer.Models
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
