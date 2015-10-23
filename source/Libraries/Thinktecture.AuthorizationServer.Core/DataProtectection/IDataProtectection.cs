/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


namespace Thinktecture.AuthorizationServer
{
    public interface IDataProtectection
    {
        byte[] Protect(byte[] data);
        byte[] Unprotect(byte[] data);
    }

    public static class DataProtectection
    {
        public static IDataProtectection Instance { get; set; }
    }
}
