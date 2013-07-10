/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer
{
    public class LocalKeyProtection : IDataProtectection
    {
        const int RequiredEncryptionKeyByteLength = 32;
        const int RequiredSigningKeyByteLength = 128;
        
        const int SignatureByteLength = 64;

        public static void CreateKeys(out string encrKey, out string signKey)
        {
            var encryptKeyBytes = IdentityModel.CryptoRandom.CreateRandomKey(RequiredEncryptionKeyByteLength);
            encrKey = encryptKeyBytes.Select(x => x.ToString("X2")).Aggregate((x, y) => x + y);

            var signKeyBytes = IdentityModel.CryptoRandom.CreateRandomKey(RequiredSigningKeyByteLength);
            signKey = signKeyBytes.Select(x => x.ToString("X2")).Aggregate((x, y) => x + y);
        }

        byte[] encryptionKey;
        byte[] signingKey;

        public LocalKeyProtection(string confidentialityKey, string validationKey)
        {
            if (String.IsNullOrWhiteSpace(confidentialityKey)) throw new ArgumentNullException("confidentialityKey");
            if (confidentialityKey.Length != RequiredEncryptionKeyByteLength * 2) throw new ArgumentException("Invalid Confidentiality Key. It must be 256 bits or 64 hex characters.");

            if (String.IsNullOrWhiteSpace(validationKey)) throw new ArgumentNullException("validationKey");
            if (validationKey.Length != RequiredSigningKeyByteLength * 2) throw new ArgumentException("Invalid Confidentiality Key. It must be 128 bytes or 256 hex characters.");

            this.encryptionKey = BytesFromHexString(confidentialityKey);
            if (this.encryptionKey == null) throw new ArgumentException("Invalid Confidentiality Key. It must be 256 bits or 64 hex characters.");
            
            this.signingKey = BytesFromHexString(validationKey);
            if (this.signingKey == null) throw new ArgumentException("Invalid Confidentiality Key. It must be 128 bytes or 256 hex characters.");
        }

        public byte[] Protect(byte[] data)
        {
            if (data == null || data.Length == 0) throw new ArgumentNullException("data");

            var cipher = Encrypt(this.encryptionKey, data);
            var sig = Sign(this.signingKey, cipher);

            if (SignatureByteLength != sig.Length) throw new Exception("Signature wrong length");

            var cipherByteLength = cipher.Length;
            var buf = new byte[SignatureByteLength + cipherByteLength];
            Array.Copy(sig, buf, SignatureByteLength);
            Array.Copy(cipher, 0, buf, SignatureByteLength, cipherByteLength);
            
            return buf;
        }

        public byte[] Unprotect(byte[] data)
        {
            if (data == null || data.Length == 0) throw new ArgumentNullException("data");
            
            var cipherLength = data.Length - SignatureByteLength;
            var sig = new byte[SignatureByteLength];
            var cipher = new byte[cipherLength];
            Array.Copy(data, sig, SignatureByteLength);
            Array.Copy(data, SignatureByteLength, cipher, 0, cipherLength);

            if (!ValidateSignature(this.signingKey, cipher, sig)) return null;
            return Decrypt(this.encryptionKey, cipher);
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

        static byte[] Sign(byte[] signingKey, byte[] message)
        {
            var hmac = new System.Security.Cryptography.HMACSHA512(signingKey);
            var hash = hmac.ComputeHash(message);
            return hash;
        }

        static bool ValidateSignature(byte[] signingKey, byte[] message, byte[] signature)
        {
            var computedSignature = Sign(signingKey, message);
            return SafeCompare(computedSignature, signature);
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        static bool SafeCompare(byte[] first, byte[] second)
        {
            if (first == null && second == null) return true;
            if (first == null || second == null) return false;
            if (first.Length != second.Length) return false;

            bool same = true;
            for (int i = 0; i < first.Length; i++)
            {
                same &= first[i] == second[i];
            }
            return same;
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
