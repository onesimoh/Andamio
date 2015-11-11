using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Andamio
{
    /// <summary>
    /// An object representing a key-value pair.
    /// </summary>    
    [XmlType("KVP")]
    [DebuggerDisplay("{Key}: {Value}")]
    [Serializable]
    public class KeyValuePair
    {
        #region Constructors
        /// <summary>
        /// Default constructor required for XmlSerializer.
        /// </summary>
        public KeyValuePair()
        {
        }

        public KeyValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }
        #endregion

        #region Conversion
        public static implicit operator KeyValuePair<string, string>(KeyValuePair kvp)
        {
            return new KeyValuePair<string, string>(kvp.Key, kvp.Value);
        }

        #endregion

        #region Properties
        [XmlAttribute]
        public string Key { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
        #endregion

        #region Format
        public string Format()
        {
            return String.Format("{0}={1}", Key, Value);
        }
        public override string ToString()
        {
            return Format();
        }

        public static KeyValuePair Parse(string value)
        {
            KeyValuePair kvp = new KeyValuePair();
            if (!String.IsNullOrEmpty(value))
            {
                string[] criteriaData = value.Split('=');
                if (criteriaData.Length > 2)
                { throw new FormatException(); }
                
                kvp.Key = criteriaData[0];
                if (criteriaData.Length == 2)
                { kvp.Value = criteriaData[1]; }
            }

            return kvp;
        }
        #endregion

        #region Comparer
        public static readonly KeyValuePairDefaultComparer DefaultComparer = new KeyValuePairDefaultComparer();
        public class KeyValuePairDefaultComparer : IEqualityComparer<KeyValuePair>
        {
            public bool Equals(KeyValuePair left, KeyValuePair right)
            {
                return left.Key.Equals(right.Key);
            }

            public int GetHashCode(KeyValuePair kvp)
            {
                return kvp.Key.GetHashCode();
            }
        }

        #endregion
    }
}
