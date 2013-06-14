using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models
{
    public class ClientModel
    {
        [Required]
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        [Required]
        public string Name { get; set; }
        public OAuthFlow Flow { get; set; }
        public bool AllowRefreshToken { get; set; }
        public bool RequireConsent { get; set; }
        public bool Enabled { get; set; }
    }
}