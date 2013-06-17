/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.ComponentModel.DataAnnotations;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models
{
    public class ScopeModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
    }
}