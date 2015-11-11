using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace Andamio.Cryptography
{
    public class CryptoKey : Disposable
    {
        #region Fields
        public static readonly int DefaultSize = 32;
        protected readonly SecureString _protectedKey;

        #endregion

        #region Constructors
        public CryptoKey() : this(CryptoUtils.GetRandom(new byte[DefaultSize]))
        {            
        }
        public CryptoKey(byte[] key)
        {
            if (key == null || key.Length == 0)
            { throw new ArgumentException("key"); }

            _protectedKey = CryptoUtils.ProtectString(Convert.ToBase64String(key));
            KeySize = key.Length;
        }

        #endregion

        #region Properties
        public byte[] Value
        {
            get 
            { 
                return Convert.FromBase64String(_protectedKey.UnprotectString());
            }
        }

        public int KeySize { get; private set; }

        #endregion

        #region Dispose
        protected override void Cleanup()
        {
            if (_protectedKey != null)
            { _protectedKey.Dispose(); }
        }

        #endregion
    }
}
