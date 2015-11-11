using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;
using Andamio.Collections;

using iTextSharp.text;
using iText = iTextSharp.text;
using iTextPdf = iTextSharp.text.pdf;

namespace Andamio.Pdf
{
    #region TableCellAlign
    public enum TableCellAlign
    {
        Left = 0,
        Center = 1,
        Right = 2,
        Justified = 3,
        Top = 4,
        Middle = 5,
        Bottom = 6,
        Baseline = 7,
        JustifiedAll = 8,
    }

    #endregion


    public class TableCells : CollectionBase<TableCell>
    {
        #region Constructors
        public TableCells()
            : base()
        {
        }

        public TableCells(IEnumerable<TableCell> cells)
            : base(cells)
        {
        }

        #endregion

        #region Serialization
        public void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            var cells = element.Elements();
            cells.ForEach(c => Add(TableCell.CreateFromXElement(c)));
        }

        #endregion
    }


    public class TableHeaderCell : TableCell
    {
        #region Constructors
        public TableHeaderCell()
            : base()
        {
        }

        #endregion
    }


    public class TableCell : PdfElement
    {
        #region Constructors
        public TableCell() : base()
        {
            Style = new TableCellStyle();
        }

        public TableCell(Table table) : this()
        {
            if (table == null)
            { throw new ArgumentNullException("table"); }
            Content = table;
        }

        public TableCell(Phrase phrase) : this()
        {
            if (phrase == null)
            { throw new ArgumentNullException("phrase"); }
            Content = phrase;
        }

        public TableCell(string text) : this(new Phrase(text))
        {
        }

        #endregion

        #region Table
        public Table ParentTable { get; internal set; }

        #endregion

        #region Content
        public IPdfElement Content { get; set; }
        public IPdfElement[] GetAllElementsRecursive()
        {
            PdfElements elements = new PdfElements();
            if (Content != null)
            { elements.Add(Content); }

            return elements.GetAllRecursive();
        }

        #endregion

        #region Style
        public TableCellStyle Style { get; private set; }

        #endregion

        #region Serialization
        internal static TableCell CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            TableCell cell;
            if (element.Name.Equals(xmlns + "Header"))
            {
                cell = new TableHeaderCell();
            }
            else if (element.Name.Equals(xmlns + "Cell"))
            {
                cell = new TableCell();
            }
            else
            {
                throw new NotSupportedException(String.Format("'{0}' Table Cell Type Not Supported.", element.Name));
            }

            if (element.Elements().Any())
            {
                if (element.Elements().Count() > 1)
                { throw new ApplicationException(); }

                XElement cellContentElement = element.Elements().First();                
                if (!PdfElementFactory.IsValidPdfElement(cellContentElement))
                { throw new ApplicationException(""); }

                cell.Content = PdfElementFactory.CreateFromXElement(cellContentElement);
            }
            else
            {
                if (!element.Value.IsNullOrBlank())
                { cell.Content = new Phrase(element.Value); }
            }

            cell.Style.PopulateFromXElement(element);

            return cell;
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
            iTextPdf.PdfPCell cell;
            if ((Content != null) && (Content is Phrase))
            {
                cell = new iTextPdf.PdfPCell((iText.Phrase) Content.GeneratePdfElement());
            }
            else
            {
                cell = new iTextPdf.PdfPCell();
                if (Content != null)
                {
                    cell.AddElement(Content.GeneratePdfElement());
                }
            }

            TableCellStyle style = (Manifest != null) ? Manifest.Styles.GetMergedFromConfiguration(Style) : Style;

            cell.BackgroundColor = new BaseColor(style.BackgroundColor ?? TableCellStyle.Default.BackgroundColor.Value);
            cell.BorderColor = new BaseColor(style.BorderColor ?? TableCellStyle.Default.BorderColor.Value);
            cell.Border = iText.Rectangle.BOX;
            cell.Padding = style.Padding ?? TableCellStyle.Default.Padding.Value;
            cell.BorderWidth = style.BorderWidth ?? TableCellStyle.Default.BorderWidth.Value;
            cell.HorizontalAlignment = (int) (style.HorizontalAlignment ?? TableCellStyle.Default.HorizontalAlignment.Value);
            cell.VerticalAlignment = (int) (style.VerticalAlignment ?? TableCellStyle.Default.VerticalAlignment.Value);          

            return cell;
        }

        #endregion
    }
}
