/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.ComponentModel.DataAnnotations;
namespace Thinktecture.AuthorizationServer.Models
{
    public class RedirectUri
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Uri { get; set; }
        public string Description { get; set; }
    }
}
