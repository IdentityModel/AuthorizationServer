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
        public virtual int ID { get; set; }
        [Required]
        public virtual string AuthorizationServerName { get; set; }
        public virtual string AuthorizationServerLogoUrl { get; set; }
        [Required]
        public virtual string Issuer { get; set; }

        public virtual List<AuthorizationServerAdministrator> Administrators { get; set; }
    }

    public class AuthorizationServerAdministrator
    {
        public int ID { get; set; }
        public string NameID { get; set; }
    }
}
