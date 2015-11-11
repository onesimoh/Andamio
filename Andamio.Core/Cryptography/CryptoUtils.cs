using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;

namespace Andamio.Cryptography
{
    public static class CryptoUtils
    {
        #region String Protection
        public static SecureString ProtectString(string data)
        {
            SecureString secureString = new SecureString();            
            if (!String.IsNullOrEmpty(data))
            {
                foreach (char c in data)
                {
                    secureString.AppendChar(c);
                }
            }
            secureString.MakeReadOnly();
            return secureString;
        }

        public static string UnprotectString(this SecureString secureString)
        {
            string unsecuredString = null;
            if (secureString != null)
            {
                using (SecureString copiedSecureString = secureString.Copy())
                {
	                IntPtr ptr = Marshal.SecureStringToBSTR(secureString);
	                unsecuredString = Marshal.PtrToStringBSTR(ptr);
	                Marshal.ZeroFreeBSTR(ptr);
                }
            }
            return unsecuredString;
        }

        #endregion

        #region Encryption (Symmetric Algorithm)
        public static byte[] EncryptData(byte[] data, out CryptoKey cryptoKey, out CryptoIV cryptoIV)
        {
            cryptoKey = new CryptoKey();
            cryptoIV = new CryptoIV();            
            return EncryptData(cryptoKey, cryptoIV, data);
        }

        public static byte[] EncryptData(CryptoKey cryptoKey, CryptoIV cryptoIV, byte[] data) 
        {
            // Advanced Encryption Standard            
            ICryptoTransform encryptor = new RijndaelManaged().CreateEncryptor(cryptoKey, cryptoIV);
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length); 
                    cryptoStream.FlushFinalBlock();  
                    return stream.ToArray(); 
                }
            }
        }

        public static byte[] DecryptData(CryptoKey cryptoKey, CryptoIV cryptoIV, byte[] data) 
        { 
            // Advanced Encryption Standard
            ICryptoTransform decryptor = new RijndaelManaged().CreateDecryptor(cryptoKey, cryptoIV);
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length); 
                    cryptoStream.FlushFinalBlock();              
                    return stream.ToArray(); 
                }
            }
        }

        public static ICryptoTransform CreateEncryptor(this SymmetricAlgorithm symmetricAlgorithm, CryptoKey cryptoKey, CryptoIV cryptoIV)
        {
            return symmetricAlgorithm.CreateEncryptor(cryptoKey.Value, cryptoIV.Value);
        }
        public static ICryptoTransform CreateDecryptor(this SymmetricAlgorithm symmetricAlgorithm, CryptoKey cryptoKey, CryptoIV cryptoIV)
        {
            return symmetricAlgorithm.CreateDecryptor(cryptoKey.Value, cryptoIV.Value);
        }

        #endregion

        #region Hashing
        public static byte[] GetMD5Hash(string value)
        {
            if (value == null)
            { throw new ArgumentNullException("value"); }

            MD5CryptoServiceProvider md5HashCSP = new MD5CryptoServiceProvider();
            md5HashCSP.ComputeHash(value.ToByteArray());
            return md5HashCSP.Hash;
        }
        public static string GetMD5HashString(byte [] bytes)
        {
            MD5CryptoServiceProvider md5HashCSP = new MD5CryptoServiceProvider();
            md5HashCSP.ComputeHash(bytes);
            return md5HashCSP.Hash.ToHexString();
        }

        public static byte[] GetMD5Hash(Stream inputStream)
        {
            MD5CryptoServiceProvider md5HashCSP = new MD5CryptoServiceProvider();
            md5HashCSP.ComputeHash(inputStream);
            return md5HashCSP.Hash;
        }
        public static string GetMD5HashString(Stream inputStream)
        {
            inputStream.Seek(0, SeekOrigin.Begin);
            inputStream.Position = 0;
            MD5CryptoServiceProvider md5HashCSP = new MD5CryptoServiceProvider();
            md5HashCSP.ComputeHash(inputStream);
            inputStream.Seek(0, SeekOrigin.Begin);
            inputStream.Position = 0;

            return md5HashCSP.Hash.ToHexString();
        }
        public static string GetMD5HashString(FileInfo fileInfo)
        {
            using (FileStream fileStream = fileInfo.OpenRead())
            {
                return GetMD5HashString(fileStream);
            }
        }
        public static byte[] GetMD5Hash(FileInfo fileInfo)
        {
            using (FileStream fileStream = fileInfo.OpenRead())
            {
                return GetMD5Hash(fileStream);
            }
        }

        public static byte[] GenerateMAC(CryptoKey validationKey, byte[] data) 
        { 
            HMACSHA256 hmac = new HMACSHA256(validationKey.Value);  
            return hmac.ComputeHash(data); 
        } 
        public static bool ValidateMAC(CryptoKey validationKey, byte[] storedMAC, byte[] data) 
        {
            byte[] mac = GenerateMAC(validationKey, data);
            return mac.SequenceEqual(storedMAC);             
        }

        #endregion

        #region Password Protection
        internal static readonly int DefaultHashSize = 32;
        public static byte[] ProtectPassword(string password, CryptoKey cryptoKey)
        {
            if (String.IsNullOrEmpty(password))
            { throw new ArgumentNullException("password"); }

            if (cryptoKey == null)
            { throw new ArgumentNullException("cryptoKey"); }

            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(password, cryptoKey.Value);
            return rdb.GetBytes(DefaultHashSize);
        }
        public static byte[] ProtectPassword(SecureString password, CryptoKey cryptoKey)
        {
            return ProtectPassword(password.UnprotectString(), cryptoKey);
        }

        public static bool ValidatePassword(string password, CryptoKey cryptoKey, byte[] hashedPassword)
        {
            return ValidatePassword(password, cryptoKey.Value, hashedPassword);
        }
        public static bool ValidatePassword(string password, byte[] cryptoKey, byte[] hashedPassword)
        {
            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(password, cryptoKey);
            byte[] hash = rdb.GetBytes(DefaultHashSize);
            return hash.SequenceEqual(hashedPassword);
        }

        public static CryptoKey GenerateKeyFromPassword(string password)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, null);  
            return new CryptoKey(pdb.GetBytes(CryptoKey.DefaultSize));
        }

        #endregion

        #region Random
        public static string GetRandomBase64String()
        {	        
            return Convert.ToBase64String(GetRandom(new byte[64]));
        }
        public static string GetRandomBase32String()
        {
            return Convert.ToBase64String(GetRandom(new byte[32]));
        }
        public static string GetRandomBase16String()
        {
            return Convert.ToBase64String(GetRandom(new byte[16]));
        }
        public static UInt64 GetRandomKey()
        {
            return BitConverter.ToUInt64(new byte[256], 0);
        }

        public static byte[] GetRandom(byte[] random)
        {
            if (random == null)
            { throw new ArgumentNullException("random"); }
            if (random.Length == 0)
            { throw new ArgumentException("", "random"); }

	        new RNGCryptoServiceProvider().GetBytes(random);
            return random;
        }

        #endregion
    }
}
