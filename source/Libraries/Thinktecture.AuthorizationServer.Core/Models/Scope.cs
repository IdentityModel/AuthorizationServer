/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Thinktecture.AuthorizationServer.Models
{
    public class Scope
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }

        public List<Client> AllowedClients { get; set; }
    }
}
