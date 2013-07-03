using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Thinktecture.AuthorizationServer.DataProtectection
{
    public class MachineKeyProtection : IDataProtectection
    {
        public byte[] Protect(byte[] data)
        {
            return MachineKey.Protect(data, "AuthorizationServer");
        }

        public byte[] Unprotect(byte[] data)
        {
            return MachineKey.Unprotect(data, "AuthorizationServer");
        }
    }
}
