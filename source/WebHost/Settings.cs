using System.Configuration;

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