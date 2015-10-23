/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.IO;

namespace Thinktecture.AuthorizationServer.WebHost
{
    public class DataProtectionConfig
    {
        public static void Configure()
        {
            //ConfigureMachineKey();
            ConfigureLocalKey();
        }

        private static void ConfigureMachineKey()
        {
            DataProtectection.Instance = new MachineKeyProtection();
        }

        private static void ConfigureLocalKey()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/dataProtectionKeys.json");
            DataProtectection.Instance = new KeyFileProtection(path);
        }

        public static void ConfigureNoKeyForTesting()
        {
            DataProtectection.Instance = new NoProtection();
        }
    }
}