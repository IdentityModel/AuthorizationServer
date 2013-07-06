/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer
{
    public class LocalKeyProtection : IDataProtectection
    {
        public const string AppSettingsConfigurationKey = "authz:protectionKey";

        byte[] cipherKey;
        public LocalKeyProtection()
            : this(ConfigurationManager.AppSettings[AppSettingsConfigurationKey])
        {
        }

        public LocalKeyProtection(string cipherHexKey)
        {
            if (String.IsNullOrWhiteSpace(cipherHexKey)) throw new ArgumentNullException("cipherHexKey");

            this.cipherKey = BytesFromHexString(cipherHexKey);
            if (this.cipherKey.Length * 8 != 256) throw new ArgumentException("cipherHexKey must be 256 bits or 64 hex characters");
        }

        public byte[] Protect(byte[] data)
        {
            return Encrypt(this.cipherKey, data);
        }

        public byte[] Unprotect(byte[] data)
        {
            return Decrypt(this.cipherKey, data);
        }

        private static byte[] Encrypt(byte[] cipherKey, byte[] plaintext)
        {
            var cipher = new System.Security.Cryptography.AesManaged();
            cipher.Key = cipherKey;
            cipher.Mode = System.Security.Cryptography.CipherMode.CBC;
            cipher.Padding = System.Security.Cryptography.PaddingMode.ISO10126;

            using (var ms = new System.IO.MemoryStream())
            {
                using (var cs = new System.Security.Cryptography.CryptoStream(
                    ms, cipher.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    cs.Write(plaintext, 0, plaintext.Length);
                }

                var ciphertext = ms.ToArray();

                var message = new byte[cipher.IV.Length + ciphertext.Length];
                cipher.IV.CopyTo(message, 0);
                ciphertext.CopyTo(message, cipher.IV.Length);
                return message;
            }
        }

        private static byte[] Decrypt(byte[] cipherKey, byte[] ciphertext)
        {
            var cipher = new System.Security.Cryptography.AesManaged();
            cipher.Key = cipherKey;
            cipher.Mode = System.Security.Cryptography.CipherMode.CBC;
            cipher.Padding = System.Security.Cryptography.PaddingMode.ISO10126;

            var ivSize = cipher.IV.Length;
            var iv = new byte[ivSize];
            Array.Copy(ciphertext, iv, ivSize);
            cipher.IV = iv;

            var data = new byte[ciphertext.Length - ivSize];
            Array.Copy(ciphertext, ivSize, data, 0, data.Length);

            using (var ms = new System.IO.MemoryStream())
            {
                using (var cs = new System.Security.Cryptography.CryptoStream(
                    ms, cipher.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                }

                var plaintext = ms.ToArray();
                return plaintext;
            }
        }

        private static byte[] BytesFromHexString(string data)
        {
            if ((data == null) || ((data.Length % 2) != 0))
            {
                return null;
            }

            byte[] buffer = new byte[data.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                int num2 = HexToInt(data[2 * i]);
                int num3 = HexToInt(data[(2 * i) + 1]);
                if ((num2 == -1) || (num3 == -1))
                {
                    return null;
                }
                buffer[i] = (byte)((num2 << 4) | num3);
            }
            return buffer;
        }

        public static int HexToInt(char h)
        {
            if ((h >= '0') && (h <= '9'))
            {
                return (h - '0');
            }
            if ((h >= 'a') && (h <= 'f'))
            {
                return ((h - 'a') + 10);
            }
            if ((h >= 'A') && (h <= 'F'))
            {
                return ((h - 'A') + 10);
            }
            return -1;
        }
    }
}
