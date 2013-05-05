/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

namespace Thinktecture.AuthorizationServer.Core.Models
{
    public class RedirectUri
    {
        public int Id { get; set; }
        public string Uri { get; set; }
        public string Description { get; set; }
    }
}
