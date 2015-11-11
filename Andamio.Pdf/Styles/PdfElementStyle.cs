using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

using Andamio;
using Andamio.Collections;
using Andamio.Serialization;

namespace Andamio.Pdf
{
    #region Collection
    public class PdfElementStyleCollection : CollectionBase<PdfElementStyle>
    {
        #region Constructors
        public PdfElementStyleCollection() : base()
        {
        }

        public PdfElementStyleCollection(IEnumerable<PdfElementStyle> styles)
            : base(styles)
        {
        }
        #endregion

        #region Indexer
        public PdfElementStyle this[string keyName]
        {
            get
            {
                return this.LastOrDefault(match => match.KeyName.Equals(keyName, StringComparison.OrdinalIgnoreCase));
            }
        }

        #endregion
    }

    #endregion


    public abstract class PdfElementStyle
    {
        #region Constructors
        protected PdfElementStyle()
        {
        }

        #endregion

        #region KeyName
        public string KeyName { get; set; }
        public string GetFullyQualifiedKeyName()
        {
            return !KeyName.IsNullOrBlank() ? String.Format("{0}.{1}", this.GetType().Name, KeyName) : this.GetType().Name;
        }

        #endregion

        #region Style
        public virtual void Apply(PdfElementStyle style)
        {
            if (style == null)
            { throw new ArgumentNullException("style"); }

            XElement serializedStyle = PdfElementStyleFactory.GenerateXElement(style);
            this.PopulateFromXElement(serializedStyle);
        }

        public virtual void Merge(PdfElementStyle style)
        {
            if (style == null)
            { throw new ArgumentNullException("style"); }

        }

        #endregion

        #region Serialization
        public virtual void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            KeyName = element.Attribute("keyName").Optional();
        }

        public virtual XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name);
            
            if (!KeyName.IsNullOrBlank())
            {
                element.Add(new XAttribute("keyName", KeyName));
            }
            
            return element;
        }

        #endregion
    }
}
