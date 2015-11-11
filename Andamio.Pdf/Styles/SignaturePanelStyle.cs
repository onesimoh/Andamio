using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

namespace Andamio.Pdf
{
    public class SignaturePanelStyle : PdfPanelStyle
    {
        #region Constructors
        public SignaturePanelStyle() : base()
        {
            Font = new PdfFont();
        }

        new public static readonly SignaturePanelStyle Default = new SignaturePanelStyle()
        {
            BackgroundColor = Color.White,
            BorderColor = Color.Transparent,
            Padding = 5f,
            Leading = 0f,
            BorderWidth = 0.5f,
            TopIndent = 0f,
            BottomIndent = 0f,
            Font = PdfFont.Default,
        };

        #endregion

        #region Properties
        public PdfFont Font { get; private set; }

        #endregion

        #region Serialization
        new public static SignaturePanelStyle CreateFromXElement(XElement element)
        {
            SignaturePanelStyle signaturePanelStyle = new SignaturePanelStyle();
            signaturePanelStyle.PopulateFromXElement(element);

            return signaturePanelStyle;
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

        #endregion

        #region Merge
        public override void Merge(PdfElementStyle style)
        {
            SignaturePanelStyle signaturePanelStyle = style as SignaturePanelStyle;
            base.Merge(signaturePanelStyle);
            Font.Merge(signaturePanelStyle.Font);
        }

        #endregion
    }
}
