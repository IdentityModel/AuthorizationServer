/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models
{
    public class SymmetricKeyModel
    {
        [Required]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}