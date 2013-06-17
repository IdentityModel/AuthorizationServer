/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.ComponentModel.DataAnnotations;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models
{
    public class GlobalViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Logo { get; set; }
        [Required]
        public string Issuer { get; set; }
    }
}