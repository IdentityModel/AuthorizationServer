/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


namespace Thinktecture.AuthorizationServer
{
    public class NoProtection : IDataProtectection
    {
        public byte[] Protect(byte[] data)
        {
            return data;
        }

        public byte[] Unprotect(byte[] data)
        {
            return data;
        }
    }
}