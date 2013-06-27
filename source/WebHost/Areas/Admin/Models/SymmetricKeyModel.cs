/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.ComponentModel.DataAnnotations;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models
{
    public class SymmetricKeyModel
    {
        [Required]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}