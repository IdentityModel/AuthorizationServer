/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.ComponentModel.DataAnnotations;
namespace Thinktecture.AuthorizationServer.Models
{
    public class ClientRedirectUri
    {
        [Key]
        public virtual int ID { get; set; }
        [Required]
        public virtual string Uri { get; set; }
        public virtual string Description { get; set; }
    }
}
