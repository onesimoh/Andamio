using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using Andamio;

namespace Andamio.Pdf
{
    #region H[1..6]
    public class H1Style : HeaderStyle
    {
        public H1Style()
            : base()
        {
        }
    }

    public class H2Style : HeaderStyle
    {
        public H2Style()
            : base()
        {
        }
    }

    public class H3Style : HeaderStyle
    {
        public H3Style()
            : base()
        {
        }
    }

    public class H4Style : HeaderStyle
    {
        public H4Style()
            : base()
        {
        }
    }

    public class H5Style : HeaderStyle
    {
        public H5Style()
            : base()
        {
        }
    }

    public class H6Style : HeaderStyle
    {
        public H6Style()
            : base()
        {
        }
    }

    #endregion


    public class HeaderStyle : PdfElementStyle
    {
        #region Constructors
        public HeaderStyle() : base()
        {
            Font = new PdfFont();
        }

        #endregion

        #region Properties
        public PdfFont Font { get; private set; }

        #endregion

        #region Serialization
        public static HeaderStyle CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            HeaderStyle headerStyle;
            if (element.Name.Equals(xmlns + PdfElementKind.H1.DisplayName()))
            {
                headerStyle = new H1Style();
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.H2.DisplayName()))
            {
                headerStyle = new H2Style();
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.H3.DisplayName()))
            {
                headerStyle = new H3Style();
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.H4.DisplayName()))
            {
                headerStyle = new H4Style();
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.H5.DisplayName()))
            {
                headerStyle = new H5Style();
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.H6.DisplayName()))
            {
                headerStyle = new H6Style();
            }
            else
            {
                throw new NotSupportedException(String.Format("'{0}' Header Style Type Not Supported.", element.Name.LocalName));
            }

            headerStyle.PopulateFromXElement(element);

            return headerStyle;
        }

        public override void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            base.PopulateFromXElement(element);
            Font.PopulateFromXElement(element);
        }

        public override XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = base.GenerateXElement(name);

            XElement fontElement = Font.GenerateXElement(xmlns + "Font");
            element.Add(fontElement.Attributes());
             
            return element;
        }

        public XElement GenerateXElement(XNamespace xmlNamespace)
        {
            XNamespace xmlns = xmlNamespace ?? XNamespace.None;

            XElement element;
            if (this is H1Style)
            {
                element = GenerateXElement(xmlns + PdfElementKind.H1.DisplayName());
            }
            else if (this is H2Style)
            {
                element = GenerateXElement(xmlns + PdfElementKind.H2.DisplayName());
            }
            else if (this is H3Style)
            {
                element = GenerateXElement(xmlns + PdfElementKind.H3.DisplayName());
            }
            else if (this is H4Style)
            {
                element = GenerateXElement(xmlns + PdfElementKind.H4.DisplayName());
            }
            else if (this is H5Style)
            {
                element = GenerateXElement(xmlns + PdfElementKind.H5.DisplayName());
            }
            else if (this is H6Style)
            {
                element = GenerateXElement(xmlns + PdfElementKind.H6.DisplayName());
            }
            else
            {
                throw new NotSupportedException(String.Format("'{0}' Header Style Type Not Supported.", this.GetType().Name));
            }

            return element;
        }
        #endregion

        #region Merge
        public override void Merge(PdfElementStyle style)
        {
            HeaderStyle headerStyle = style as HeaderStyle;
            base.Merge(headerStyle);

            Font.Merge(headerStyle.Font);
        }

        #endregion
    }
}
