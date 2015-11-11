using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Andamio.Xml
{
    /// <summary>
    /// Writes Xml fragments (no xml declaration or xml namespace attributes).
    /// </summary>
    public class XmlFragmentWriter : XmlTextWriter
    {
        #region Constructors
        /// <summary>
        /// Creates a XmlFragmentWriter object.
        /// </summary>
        /// <param name="w">The text writer to write to.</param>
        public XmlFragmentWriter(TextWriter w) : base(w)
        {            
        }
        #endregion

        #region XmlTextWriter Overrides
        private bool _skipAttribute = false;
        public const string Xmlns = "xmlns";
        public const string Xsd = "xsd";
        public const string Xsi = "xsi";

        /// <summary>
        /// Writes the start of an attribute. 
        /// </summary>
        /// <param name="prefix">The namespace prefix of the attribute.</param>
        /// <param name="localName">The name of the attribute.</param>
        /// <param name="ns">The namespace URI of the attribute.</param>
        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            //omits XSD and XSI declarations. 
            if (prefix.Equals(Xmlns, StringComparison.OrdinalIgnoreCase) 
                && (localName.Equals(Xsd, StringComparison.OrdinalIgnoreCase) 
                || localName.Equals(Xsi, StringComparison.OrdinalIgnoreCase)))
            {
                _skipAttribute = true;
            }
            else
            {
                base.WriteStartAttribute(prefix, localName, ns);
            }
        }

        /// <summary>
        /// Writes the given text content.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public override void WriteString(string text)
        {
            if (!_skipAttribute)
            {
                base.WriteString(text);
            }
        }

        /// <summary>
        /// Writes the end of an attribute.
        /// </summary>
        public override void WriteEndAttribute()
        {
            if (_skipAttribute)
            {
                _skipAttribute = false; // reset flag for next attribute 
            }
            else
            {
                base.WriteEndAttribute();
            }
        }

        /// <summary>
        /// Writes the XML declaration.
        /// </summary>
        public override void WriteStartDocument()
        {
            //don't write anything because this an XML fragment
        }
        #endregion
    }
}
