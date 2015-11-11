using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Andamio;
using Andamio.Collections;
using Andamio.Diagnostics;
using Andamio.Data.Flow.Media.Readers;

namespace Andamio.Data
{
    public class DataGrid
    {
        #region Constructors
        public DataGrid()
        {
            Columns = new DataGridColumns();
            Columns.ItemsInserted += OnColumnsInserted;
            
            Rows = new DataGridRows();
            Rows.ItemsInserted += OnRowsInserted;            
        }

        public DataGrid(DataGridColumns columns) : this()
        {
            if (columns == null) throw new ArgumentNullException("columns");
            Columns.AddRange(columns);
        }

        public DataGrid(IEnumerable<DataGridColumn> columns) : this()
        {
            if (columns == null) throw new ArgumentNullException("columns");
            Columns.AddRange(columns);
        }

        #endregion

        #region Columns
        public DataGridColumns Columns { get; private set; }
        void OnColumnsInserted(object sender, ItemEventArgs<IEnumerable<DataGridColumn>> e)
        {
            var columns = e.Item;
            if (columns != null)
            { columns.ForEach(c => c.DataGrid = this); }
        }
        
        #endregion

        #region Rows
        public DataGridRows Rows  { get; private set; }
        void OnRowsInserted(object sender, ItemEventArgs<IEnumerable<DataGridRow>> e)
        {
            var rows = e.Item;
            if (rows != null)
            { rows.ForEach(r => r.DataGrid = this); }        
        }

        #endregion

        #region Cells
        public DataGridCells Cells()
        {
            return Rows.SelectMany(r => r.Cells).ToArray();
        }

        public DataGridCell Cells(int rowIndex, int columnIndex)
        {
            DataGridRow row = Rows[rowIndex];
            return row.Cells[columnIndex];
        }

        public DataGridCell Cells(int rowIndex, string columnHeader)
        {
            DataGridRow row = Rows[rowIndex];
            DataGridColumn column = Columns[columnHeader];
            return row.Cells[column.Index()];
        }

        public DataGridCell this[int rowIndex, int columnIndex]
        {
            get
            {
                return Cells(rowIndex, columnIndex);
            }
        }

        public DataGridCell this[int rowIndex, string columnHeader]
        {
            get
            {
                return Cells(rowIndex, columnHeader);
            }
        }

        public void Empty()
        {
            Rows.ForEach(r => r.Empty());
            Rows.Clear();
        }

        #endregion

        #region Xml
        public string ToXml()
        {
            throw new NotImplementedException();
        }

        public void ToXml(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void ToXml(string outputFile)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Excel
        public void ToExcel(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void ToExcel(string filePath)
        {
            using (Stream stream = File.OpenWrite(filePath))
            {
                ToExcel(stream);
            }
        }

        public IEnumerable<Row> ToExcelRows()
        {
            // Header
            if (Columns.Any())
            {
                var cells = Columns.Select(c => new Cell() { DataType = CellValues.String, CellValue = new CellValue(c.ColumnName) });
                Row row = new Row(cells) { RowIndex = 1 };
                yield return row;
            }

            // Content
            UInt32Value rowIndex = 2;
            foreach (DataGridRow dataGridRow in Rows)
            {
                Row row = new Row() { RowIndex = rowIndex };
                foreach (DataGridCell cell in dataGridRow.Cells)
                {
                    DataGridColumn column = cell.Column;
                    CellValues dataType;
                    if (column != null)
                    {                    
                        if (column.ColumnType.IsNumeric())
                        {
                            dataType = CellValues.Number;
                        }
                        else if (column.ColumnType.IsDateTime())
                        {
                            dataType = CellValues.Date;
                        }
                        else
                        {
                            dataType = CellValues.String;
                        }
                    }
                    else
                    {
                        dataType = CellValues.String;
                    }

                    row.Append(new Cell() { DataType = dataType, CellValue = new CellValue(cell.Format()) });
                }

                rowIndex++;

                yield return row;
            }
        }

        public void ToExcelSheet(SheetData sheetData)
        {
            if (sheetData == null) throw new ArgumentNullException("sheetData");
            this.ToExcelRows().ForEach(dataRow => sheetData.AppendChild(dataRow));
        }

        public void ToExcelSheet(Worksheet worksheet)
        {
            if (worksheet == null) throw new ArgumentNullException("worksheet");
            SheetData sheetData = worksheet.Descendants<SheetData>().First();
            ToExcelSheet(sheetData);
        }

        #endregion

        #region Html
        public string ToHtml()
        {            
            using (MemoryStream stream = new MemoryStream())
            {
                ToHtml(stream);
                //stream.Position = 0;
                //stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }                
            }
        }

        public void ToHtml(Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
            {
                writer.WriteLine("<table border=\"1\">");

                if (Columns.Any())
                {
                    writer.WriteLine("<thead>");
                    writer.WriteLine("<tr>");
                    Columns.ForEach(delegate(DataGridColumn column) {
                        writer.WriteLine("<th>");
                        writer.WriteLine(column.BindingName);
                        writer.WriteLine("</th>");
                    });
                    writer.WriteLine("</tr>");
                    writer.WriteLine("</thead>");
                }

                if (Rows.Any())
                {
                    writer.WriteLine("<tbody>");
                    Rows.ForEach(delegate(DataGridRow row) {
                        writer.WriteLine("<tr>");
                        row.Cells.ForEach(delegate(DataGridCell cell) {
                            writer.WriteLine("<td>");
                            writer.WriteLine(cell.OriginalValue);
                            writer.WriteLine("</td>");
                        });
                        writer.WriteLine("</tr>");
                    });
                    writer.WriteLine("</tbody>");
                }
                writer.WriteLine("</table>");
            }
        }

        public void ToHtml(string filePath)
        {
            using (Stream stream = File.OpenWrite(filePath))
            {
                ToHtml(stream);
            }
        }

        #endregion

        #region CSV
        public string ToCSV(string delimiter = ",", string defaultNullOrEmpty = "")
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Header
            stringBuilder.AppendLine(Columns.Select(c => String.Format("\"{0}\"", c.BindingName)).JoinStrings(delimiter));

            // Rows
            Rows.ForEach(delegate(DataGridRow row)
            {
                stringBuilder.AppendLine(row.Cells.Select(c => String.Format("\"{0}\"", (c.OriginalValue != null) ? c.OriginalValue : defaultNullOrEmpty))
                    .ToArray().JoinStrings(delimiter, ignoreNullOrEmpty: false));
            });

            return stringBuilder.ToString();
        }

        public void ToCSV(string outputFile, string delimiter = ",", string defaultNullOrEmpty = "")
        {
            if (outputFile.IsNullOrBlank()) throw new ArgumentNullException("outputFile");
            
            if (File.Exists(outputFile))
            { File.Delete(outputFile); }

            string directory = Path.GetDirectoryName(outputFile);
            if (!directory.IsNullOrBlank() && !Directory.Exists(directory))
            { Directory.CreateDirectory(directory); }

            using (FileStream stream = File.OpenWrite(outputFile))
            {
                using (StreamWriter outfile = new StreamWriter(stream))
                {
                    outfile.Write(ToCSV(delimiter, defaultNullOrEmpty));
                }
            }
        }

        #endregion

        #region Read
        public virtual void Read(ImportReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            reader.Populate(this);            
        }

        #endregion
    }
}
