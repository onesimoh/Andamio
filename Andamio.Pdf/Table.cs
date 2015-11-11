using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

using iTextPdf = iTextSharp.text.pdf;

namespace Andamio.Pdf
{
    public class Table : PdfElement
    {
        #region Constructors
        public Table() : base()
        {
            Style = new TableStyle();
            Rows = new TableRows();
            Rows.ItemsInserted += OnRowsInserted;
        }

        #endregion

        #region Rows, Cells
        public TableRows Rows { get; private set; }
        void OnRowsInserted(object sender, ItemEventArgs<IEnumerable<TableRow>> e)
        {
            var rows = e.Item;
            if (rows != null)
            {
                foreach (TableRow row in rows)
                {
                    row.Manifest = this.Manifest;
                    row.ParentTable = this;
                }
            }
        }

        public TableCell[] GetAllCells()
        {
            return Rows.SelectMany(r => r.Cells).ToArray();
        }

        #endregion

        #region Style
        public TableStyle Style { get; private set; }

        #endregion

        #region Serialization
        internal static Table CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            Table table = new Table();
            table.Style.PopulateFromXElement(element);
            table.Rows.PopulateFromXElement(element);

            return table;
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
            TableStyle style = (Manifest != null) ? Manifest.Styles.GetMergedFromConfiguration(Style) : Style;

            int columnCount = (Rows.Any()) ? Rows.First().Cells.Count : 0;
            iTextPdf.PdfPTable table = new iTextPdf.PdfPTable(columnCount)
            {
                HorizontalAlignment = (int) (style.HorizontalAlignment ?? TableStyle.Default.HorizontalAlignment.Value),
                SpacingBefore = style.SpacingBefore ?? TableStyle.Default.SpacingBefore.Value,
                SpacingAfter = style.SpacingAfter ?? TableStyle.Default.SpacingAfter.Value,
                LockedWidth = style.LockedWidth ?? TableStyle.Default.LockedWidth.Value,
            };

            Rows.SelectMany(r => r.Cells).ForEach(c => table.AddCell((iTextPdf.PdfPCell) c.GeneratePdfElement()));

            if (style.Widths != null && style.Widths.Any())
            {
                table.SetTotalWidth(style.Widths);
            }
            if (style.WidthPercentage.HasValue)
            {
                table.WidthPercentage = style.WidthPercentage.Value;
            }
            
            return table;
        }

        #endregion
    }
}
