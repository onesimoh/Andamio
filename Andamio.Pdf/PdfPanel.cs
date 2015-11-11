using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

using iTextSharp.text;
using iText = iTextSharp.text;
using iTextPdf = iTextSharp.text.pdf;

namespace Andamio.Pdf
{
    public class PdfPanel : PdfContainerElement
    {
        #region Constructors
        public PdfPanel() : base()
        {
            Style = new PdfPanelStyle();
        }

        public PdfPanel(IPdfElement element) : this()
        {
            Content.Add(element);
        }

        public PdfPanel(IEnumerable<IPdfElement> elements) : this()
        {
            Content.AddRange(elements);
        }

        #endregion

        #region Style
        public PdfPanelStyle Style { get; private set; }

        #endregion

        #region Serialization
        internal static PdfPanel CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            PdfPanel panel = new PdfPanel();
            panel.Style.PopulateFromXElement(element);
            if (element.Elements().Any())
            {
                panel.Content.AddRange(element.Elements().Select(e => PdfElementFactory.CreateFromXElement(e)));
            }

            return panel;
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
            PdfPanelStyle style = (Manifest != null) ? Manifest.Styles.GetMergedFromConfiguration(Style) : Style;

            iTextPdf.PdfPTable panel = new iTextPdf.PdfPTable(1) 
            {
                HorizontalAlignment = (int)(style.HorizontalAlignment ?? PdfPanelStyle.Default.HorizontalAlignment.Value),
                SpacingBefore = style.TopIndent ?? PdfPanelStyle.Default.TopIndent.Value,
                SpacingAfter = style.BottomIndent ?? PdfPanelStyle.Default.BottomIndent.Value,
            };

            iTextPdf.PdfPCell cell = new iTextPdf.PdfPCell()
            {
                BackgroundColor = new BaseColor(style.BackgroundColor ?? PdfPanelStyle.Default.BackgroundColor.Value),
                BorderColor = new BaseColor(style.BorderColor ?? PdfPanelStyle.Default.BorderColor.Value),
                Border = iText.Rectangle.BOX,
                Padding = style.Padding ?? PdfPanelStyle.Default.Padding.Value,
                BorderWidth = style.BorderWidth ?? PdfPanelStyle.Default.BorderWidth.Value,                
            };

            if (Content.Any())
            {
                Content.ForEach(e => cell.AddElement(e.GeneratePdfElement()));
            }

            panel.AddCell(cell);

            cell.SetLeading(style.Leading ?? PdfPanelStyle.Default.Leading.Value, 0);

            if (style.Width.HasValue && !style.Width.Value.IsBlank)
            {
                PdfUnit unit = style.Width.Value;
                if (unit.Type.IsPercentage())
                {
                    panel.WidthPercentage = unit.Value;
                }
                else
                {
                    panel.SetTotalWidth(new float[] { unit.Value });
                    panel.LockedWidth= true;
                }
            }

            return panel;
        }

        #endregion
    }
}
