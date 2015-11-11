using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

using iTextSharp.text;
using iText = iTextSharp.text;
using iTextDraw = iTextSharp.text.pdf.draw;

namespace Andamio.Pdf
{
    public class Separator : PdfElement
    {
        #region Constructors
        public Separator() : base()
        {
            Style = new SeparatorStyle();
        }

        #endregion

        #region Style
        public SeparatorStyle Style { get; private set; }

        #endregion

        #region Serialization
        internal static Separator CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            Separator separator = new Separator();
            separator.Style.PopulateFromXElement(element);

            return separator;
        }

        internal XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;

            XElement element = new XElement(name);

            return element;
        }

        #endregion

        #region Pdf Element
        public override iTextSharp.text.IElement GeneratePdfElement()
        {
            SeparatorStyle style = (Manifest != null) ? Manifest.Styles.GetMergedFromConfiguration(Style) : Style;

            iTextDraw.LineSeparator separator = new iTextDraw.LineSeparator() 
            {
                LineWidth = style.Width ?? SeparatorStyle.Default.Width.Value,
                LineColor = new BaseColor(style.BorderColor ?? SeparatorStyle.Default.BorderColor.Value),
            };
            return new iText.Chunk(separator);
        }

        #endregion
    }
}
