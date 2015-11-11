using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using System.Xml;

using Andamio;
using Andamio.Collections;

namespace Andamio.Diagnostics
{
    /// <summary>
    /// Strongly Type Collection of Log Attachement entries.
    /// </summary>
    public class LogAttachments : CollectionBase<LogAttachment>
    {
        #region Constructors
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public LogAttachments() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance that contains the specified entries.
        /// </summary>
        /// <param name="logAttachements">The Log Attachement entries to initialize the Collection</param>
        public LogAttachments(IEnumerable<LogAttachment> logAttachements) : base(logAttachements)
        {
        }
        
        #endregion

        #region Serialization
        /// <summary>
        /// Serializes the Collection to Xml.
        /// </summary>
        /// <param name="name">The name of the Xml Element.</param>
        /// <returns>An XElement object.</returns>
        public XElement GenerateXml(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name, this.Select(attachment => attachment.GenerateXml(xmlns + "Attachement")));
            return element;
        }

        #endregion
    }



    /// <summary>
    /// This class encapsulates a KeyValuePair&lt;string, object&gt; used for Logging purposes.
    /// </summary>
    public class LogAttachment
    {
        #region Constructors
        /// <summary>Default Constructor</summary>
        protected LogAttachment()
        {
        }

        /// <summary>
        /// Constructs a LogAttachement object with a specified KeyValuePair&lt;string, object&gt;.
        /// </summary>
        /// <param name="kvp">The Key Value Pair to initialize the object.</param>
        public LogAttachment(KeyValuePair attachement) : this(attachement.Key, attachement.Value)
        {
        }

        /// <summary>
        /// Constructs a LogAttachement object with a specified Key and Value Pair.
        /// </summary>
        /// <param name="key">The Log Attachement Key to initialize the object.</param>
        /// <param name="key">The Log Attachement Value to initialize the object.</param>
        public LogAttachment(string key, string value) : this()
        {
            this.Key = key;
            this.Value = value;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// They Key element in the KeyValuePair.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// They Value element in the KeyValuePair.
        /// </summary>
        public string Value { get; private set; }

        #endregion

        #region Serialization
        /// <summary>
        /// Generates an Xml Element.
        /// </summary>
        /// <param name="name">The Name for the Xml Element.</param>
        /// <returns>An XElement.</returns>
        public virtual XElement GenerateXml(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name);

            element.Add(new XAttribute("Key", Key), Value);            

            return element;
        }

        #endregion
    }


    /// <summary>
    /// This class encapsulates a KeyValuePair&lt;string, object&gt; used for Logging purposes.
    /// </summary>
    public class XmlLogAttachment : LogAttachment
    {
        #region Constructors
        /// <summary>Default Constructor</summary>
        protected XmlLogAttachment() : base()
        {
        }

        /// <summary>
        /// Constructs a LogAttachement object with a specified KeyValuePair&lt;string, object&gt;.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public XmlLogAttachment(string key, XElement value) : base(key, value.GetInnerXmlString())
        {
        }

        #endregion

        #region Serialization
        /// <summary>
        /// Serializes the Xml Log Attachement instance to Xml.
        /// </summary>
        /// <param name="name">The name of the Xml Element.</param>
        /// <returns>A XElement object.</returns>
        public override XElement GenerateXml(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name);

            XElement valueElem = XElement.Parse(Value);
            element.Add(new XAttribute("Key", Key), valueElem);

            return element;
        }

        #endregion
    }
}
