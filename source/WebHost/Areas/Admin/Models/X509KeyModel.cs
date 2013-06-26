using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models
{
    public class X509KeyModel
    {
        [Required]
        public string Name { get; set; }
    }
}