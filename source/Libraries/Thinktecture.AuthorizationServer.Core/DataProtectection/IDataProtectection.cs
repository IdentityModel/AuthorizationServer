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

namespace Thinktecture.AuthorizationServer.DataProtectection
{
    public interface IDataProtectection
    {
        byte[] Protect(byte[] data);
        byte[] Unprotect(byte[] data);
    }
}
