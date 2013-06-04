using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models
{
    public class ClientRedirectModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string Uri { get; set; }
    }
}