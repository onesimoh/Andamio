using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Security;

using Andamio.Cryptography;

namespace Andamio.Security
{
    public class UserCredentials : Disposable
    {
        #region Constructors
        private UserCredentials()
        {
            IsEmpty = true;
        }

        public UserCredentials(string userName, string password)
            : this(new GenericIdentity(userName), password)
        {
        }        
        
        public UserCredentials(IIdentity identity, string password)
        {
            Identity = identity;
            Password = CryptoUtils.ProtectString(password);
        }

        #endregion

        #region Empty User Credentials
        public static readonly UserCredentials Empty = new UserCredentials();

        #endregion

        #region Properties
        public IIdentity Identity { get; private set; }
        public SecureString Password { get; private set; }
        public bool IsEmpty { get; private set; }

        #endregion

        #region Dispose
        protected override void Cleanup()
        {
            if (Password != null)
            {
                Password.Dispose();
            }
        }

        #endregion
    }
}
