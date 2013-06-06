using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class Settings
    {
        public static bool EnableAdmin
        {
            get
            {
                return ConfigurationManager.AppSettings["authz:EnableAdmin"].Equals("true");
            }
        }
    }
}