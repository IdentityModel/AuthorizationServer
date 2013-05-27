using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.AuthorizationServer.Models
{
    // highlander class
    public class GlobalConfiguration
    {
        public string AuthorizationServerName { get; set; }
        public string AuthorizationServerLogoUrl { get; set; }
        public string Issuer { get; set; }

        public List<string> Administrators { get; set; }
    }
}
