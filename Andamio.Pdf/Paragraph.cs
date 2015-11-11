using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using iText = iTextSharp.text;

namespace Andamio.Pdf
{
    public class Paragraph : PdfElement
    {
        #region Constructors
        public Paragraph() : base()
        {
            Style = new ParagraphStyle();
        }

        public Paragraph(string text) : this()
        {
            Text = text;
        }

        #endregion

        #region Properties
        public string Text { get; set; }

        #endregion

        #region Style
        public ParagraphStyle Style { get; private set; }

        #endregion

        #region Serialization
        internal static Paragraph CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            Paragraph paragraph = new Paragraph() { Text = element.Value };
            paragraph.Style.PopulateFromXElement(element);

            return paragraph;
        }

        public virtual XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;

            XElement element = new XElement(name);

            return element;
        }

         
        #endregion

        #region Pdf Element
        public override iTextSharp.text.IElement GeneratePdfElement()
        {
            ParagraphStyle style = (Manifest != null) ? Manifest.Styles.GetMergedFromConfiguration(Style) : Style;

            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph(Text, (iText.Font)style.Font)
            {
                IndentationLeft = (float)(style.IndentationLeft ?? ParagraphStyle.Default.IndentationLeft.Value),
                IndentationRight = (float)(style.IndentationRight ?? ParagraphStyle.Default.IndentationRight.Value),
                //Leading = (float)(style.Leading ?? ParagraphStyle.Default.Leading.Value),
                SpacingAfter = (float)(style.SpacingAfter ?? ParagraphStyle.Default.SpacingAfter.Value),
                SpacingBefore = (float)(style.SpacingBefore ?? ParagraphStyle.Default.SpacingBefore.Value),
            };

            return paragraph;
        }

        #endregion
    }
}
