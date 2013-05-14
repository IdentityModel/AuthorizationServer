/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
namespace Thinktecture.AuthorizationServer.Models
{
    public class Scope
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }

        public List<Client> AllowedClients { get; set; }
    }
}
