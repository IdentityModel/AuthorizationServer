/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

namespace Thinktecture.AuthorizationServer.Models
{
    public class Scope
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }

        public Clients AllowedClients { get; set; }
    }
}
