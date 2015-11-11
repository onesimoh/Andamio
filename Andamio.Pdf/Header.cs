using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using Andamio;

using iText = iTextSharp.text;

namespace Andamio.Pdf
{
    #region H[1..6]
    public class H1 : Header
    {
        public H1()
            : base()
        {
        }
        public H1(string text)
            : base(text)
        {
        }
    }

    public class H2 : Header
    {
        public H2()
            : base()
        {
        }
        public H2(string text)
            : base(text)
        {
        }
    }

    public class H3 : Header
    {
        public H3()
            : base()
        {
        }
        public H3(string text)
            : base(text)
        {
        }
    }

    public class H4 : Header
    {
        public H4()
            : base()
        {
        }
        public H4(string text)
            : base(text)
        {
        }
    }

    public class H5 : Header
    {
        public H5()
            : base()
        {
        }
        public H5(string text)
            : base(text)
        {
        }
    }

    public class H6 : Header
    {
        public H6()
            : base()
        {
        }
        public H6(string text)
            : base(text)
        {
        }
    }

    #endregion


    public abstract class Header : PdfElement
    {
        #region Constructors
        protected Header() : base()
        {
            if (this is H1)
            {
                Style = new H1Style();
            }
            else if (this is H2)
            {
                Style = new H2Style();
            }
            else if (this is H3)
            {
                Style = new H3Style();
            }
            else if (this is H4)
            {
                Style = new H5Style();
            }
            else if (this is H5)
            {
                Style = new H5Style();
            }
            else if (this is H6)
            {
                Style = new H6Style();
            }
            else
            {
                throw new NotSupportedException(String.Format("'{0}' Header Style Type Not Supported.", this.GetType().Name));
            }
        }

        protected Header(string text) : this()
        {
            Text = text;
        }

        #endregion

        #region Properties
        #endregion

        #region Text
        public string Text { get; set; }

        #endregion

        #region Style
        public HeaderStyle Style { get; private set; }

        #endregion

        #region Serialization
        public static readonly Regex PdfHeaderRegEx = new Regex("H[1-6]", RegexOptions.Singleline);

        public static Header CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            Header header;
            if (element.Name.Equals(xmlns + PdfElementKind.H1.DisplayName()))
            {
                header = new H1();
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.H2.DisplayName()))
            {
                header = new H2();
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.H3.DisplayName()))
            {
                header = new H3();
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.H4.DisplayName()))
            {
                header = new H4();
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.H5.DisplayName()))
            {
                header = new H5();
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.H6.DisplayName()))
            {
                header = new H6();
            }
            else
            {
                throw new NotSupportedException(String.Format("'{0}' Header Type Not Supported.", element.Name.LocalName));
            }

            header.Text = element.Value;            
            header.Style.PopulateFromXElement(element);

            return header;
        }

        public XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;

            XElement element = new XElement(name);

            return element;
        }

        #endregion

        #region Pdf Element
        public override iTextSharp.text.IElement GeneratePdfElement()
        {
            HeaderStyle style = (Manifest != null) ? Manifest.Styles.GetMergedFromConfiguration(Style) : Style;

            iText.Phrase phrase = new iText.Phrase(Text, (iText.Font) style.Font);
            phrase.Add(iTextSharp.text.Chunk.NEWLINE);
            return phrase;
        }

        #endregion
    }
}
