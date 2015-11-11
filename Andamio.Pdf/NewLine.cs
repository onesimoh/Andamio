using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using iText = iTextSharp.text;
using iTextPdf = iTextSharp.text.pdf;
using iTextPdfDraw = iTextSharp.text.pdf.draw;

namespace Andamio.Pdf
{
    public class NewLine : PdfElement
    {
        #region Constructors
        public NewLine() : base()
        {
        }

        #endregion

        #region Serialization
        internal static NewLine CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            NewLine lineBreak = new NewLine();
            
            return lineBreak;
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
            return iText.Chunk.NEWLINE;
        }

        #endregion
    }
}
