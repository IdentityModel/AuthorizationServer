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
        public virtual int ID { get; set; }
        [Required]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Emphasize { get; set; }

        public virtual List<Client> AllowedClients { get; set; }
    }
}
