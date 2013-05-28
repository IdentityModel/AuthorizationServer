using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.AuthorizationServer.Models
{
    // highlander class
    public class GlobalConfiguration
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string AuthorizationServerName { get; set; }
        public string AuthorizationServerLogoUrl { get; set; }
        [Required]
        public string Issuer { get; set; }

        public List<AuthorizationServerAdministrator> Administrators { get; set; }
    }

    public class AuthorizationServerAdministrator
    {
        public int ID { get; set; }
        public string NameID { get; set; }
    }
}
