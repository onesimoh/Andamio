using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;
using Andamio.Collections;

namespace Andamio.Pdf
{
    public class TableRows : CollectionBase<TableRow>
    {
        #region Constructors
        public TableRows()
            : base()
        {
        }

        public TableRows(IEnumerable<TableRow> rows)
            : base(rows)
        {
        }

        #endregion

        #region Serialization
        public void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            var rows = element.Elements(xmlns + "Row");
            rows.ForEach(r => Add(TableRow.CreateFromXElement(r)));
        }

        #endregion
    }    


    public class TableRow : PdfElement
    {
        #region Constructors
        public TableRow() : base()
        {
            Cells = new TableCells();
            Cells.ItemsInserted += OnCellsInserted;
        }

        #endregion

        #region Table
        public Table ParentTable { get; internal set; }

        #endregion

        #region Cells
        public TableCells Cells { get; private set; }
        void OnCellsInserted(object sender, ItemEventArgs<IEnumerable<TableCell>> e)
        {
            var cells = e.Item;
            if (cells != null)
            {
                foreach (TableCell cell in cells)
                {
                    cell.Manifest = this.Manifest;
                    cell.ParentTable = ParentTable;
                }
            }
        }

        #endregion

        #region Serialization
        internal static TableRow CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            TableRow row = new TableRow();
            row.Cells.PopulateFromXElement(element);

            return row;
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
            throw new InvalidOperationException();
        }

        #endregion
    }

}
