using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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