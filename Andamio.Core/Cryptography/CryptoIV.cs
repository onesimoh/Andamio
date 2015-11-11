using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;

using Andamio;

namespace Andamio.Cryptography
{
    public class CryptoIV : Disposable
    {
        #region Fields
        public static readonly int DefaultSize = 16;        
        protected readonly SecureString _protectedIV;

        #endregion

        #region Constructors
        public CryptoIV()
        {
            RijndaelManaged symmetricAlgorithm = new RijndaelManaged();
            symmetricAlgorithm.GenerateIV();
            _protectedIV = CryptoUtils.ProtectString(Convert.ToBase64String(symmetricAlgorithm.IV));
            KeySize = symmetricAlgorithm.BlockSize / 8;
        }

        public CryptoIV(byte[] cryptoIV)
        {
            if (cryptoIV == null || cryptoIV.Length == 0)
            { throw new ArgumentException("cryptoIV"); }

            _protectedIV = CryptoUtils.ProtectString(Convert.ToBase64String(cryptoIV));
            KeySize = cryptoIV.Length;
        }

        #endregion

        #region Properties
        public byte[] Value
        {
            get 
            { 
                return Convert.FromBase64String(_protectedIV.UnprotectString());
            }
        }

        public int KeySize { get; private set; }

        #endregion

        #region Dispose
        protected override void Cleanup()
        {
            if (_protectedIV != null)
            { _protectedIV.Dispose(); }
        }

        #endregion       
    }
}
