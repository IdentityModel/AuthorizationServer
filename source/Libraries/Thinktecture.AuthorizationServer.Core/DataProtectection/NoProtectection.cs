/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

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