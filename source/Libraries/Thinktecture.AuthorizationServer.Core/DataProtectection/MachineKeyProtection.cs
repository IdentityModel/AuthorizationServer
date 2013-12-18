/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Web.Security;

namespace Thinktecture.AuthorizationServer
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
