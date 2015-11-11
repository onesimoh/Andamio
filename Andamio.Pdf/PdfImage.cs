using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;

using Andamio;

using iText = iTextSharp.text;

namespace Andamio.Pdf
{
    public class PdfImage : PdfElement
    {
        #region Constructors
        public PdfImage() : base()
        {
            Style = new PdfImageStyle();
        }

        public PdfImage(string imagePath) : this()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Style
        public PdfImageStyle Style { get; private set; }
        #endregion

        #region Serialization
        internal static PdfImage CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            PdfImage image = new PdfImage();
            image.Style.PopulateFromXElement(element);

            return image;
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
            PdfImageStyle style = (Manifest != null) ? Manifest.Styles.GetMergedFromConfiguration(Style) : Style;

            PdfImageType imageType = style.Type ?? PdfImageType.File;
            if (imageType.IsResource())
            { throw new NotImplementedException(); }

            if (style.Source.IsNullOrBlank())
            { return new iText.Chunk(); }

            iText.Image image = iText.Image.GetInstance(style.Source);
            image.ScaleAbsolute(image.Width/2,  image.Height/2);
            return new iText.Phrase(new iText.Chunk(image, 0f, 0f)) { Leading = 5f };
        }

        #endregion
    }
}
