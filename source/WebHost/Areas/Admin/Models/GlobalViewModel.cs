using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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