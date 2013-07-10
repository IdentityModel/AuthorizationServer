using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

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
            string encrKey, signKey;
            InitializeAndGetLocalKeys(out encrKey, out signKey);
            DataProtectection.Instance = new LocalKeyProtection(encrKey, signKey);
        }

        public static void ConfigureNoKeyForTesting()
        {
            DataProtectection.Instance = new NoProtection();
        }
        
        public static void ConfigureNewLocalKey()
        {
            string encrKey, signKey;
            CreateKeyFile(out encrKey, out signKey);
            DataProtectection.Instance = new LocalKeyProtection(encrKey, signKey);
        }

        private static string GetConfigPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/dataProtectionKeys.json");
        }

        private static void InitializeAndGetLocalKeys(out string encrKey, out string signKey)
        {
            var path = GetConfigPath();
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var keys = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(json, new { encryptKey = "", signKey = "" });
                encrKey = keys.encryptKey;
                signKey = keys.signKey;
            }
            else
            {
                CreateKeyFile(out encrKey, out signKey);
            }
        }

        private static void CreateKeyFile(out string encrKey, out string signKey)
        {
            LocalKeyProtection.CreateKeys(out encrKey, out signKey);

            var keys = new { encryptKey = encrKey, signKey = signKey };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(keys);
            var path = GetConfigPath();
            File.WriteAllText(path, json);
        }
    }
}