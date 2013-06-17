/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.ComponentModel.DataAnnotations;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models
{
    public class ClientRedirectModel
    {
        [Required]
        public string Uri { get; set; }
        public string Description { get; set; }
    }
}