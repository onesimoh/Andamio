using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Andamio;
using Andamio.Cryptography;

namespace Andamio.Security
{
    [Serializable]
    [DebuggerDisplay("{Text}")]  
    public class Signature : ISerializable
    {
        #region Constructors
        private Signature()
        {
        }
        
        public Signature(string text) : this()
        {
            if (text.IsNullOrBlank())
            { throw new ArgumentNullException("text"); }

            Text = text;
            SecurityHash = CryptoUtils.GetMD5Hash(text);
            DateTime = DateTime.UtcNow;
        }

        public Signature(string text, byte[] securityToken) : this(text)
        {
            SecurityToken = securityToken;
        }

        #endregion

        #region Properties
        public string Text { get; private set; }
        public DateTime DateTime { get; set; }
        public byte[] SecurityHash { get; private set; }
        public byte[] SecurityToken { get; private set; }
        public string UserName { get; set; }

        #endregion

        #region ISerializable Members
        public Signature(SerializationInfo info, StreamingContext context)
        {
            Text = info.GetString("Text");
            DateTime = info.GetDateTime("DateTime");
            SecurityHash = (byte[]) info.GetValue("SecurityHash", typeof(byte[]));
            SecurityToken = (byte[])info.GetValue("SecurityToken", typeof(byte[]));
            UserName = info.GetString("UserName");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Text", Text);
            info.AddValue("DateTime", DateTime);
            info.AddValue("SecurityHash", SecurityHash);
            info.AddValue("SecurityToken", SecurityToken);
            info.AddValue("UserName", UserName);
        }

        #endregion
    }
}
