using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Andamio;
using Andamio.Data;
using Andamio.Diagnostics;
using Andamio.Excel.Interop;

using ExcelInterop = Microsoft.Office.Interop.Excel;

namespace Andamio.Data.Flow.Media.Readers
{
    #region Settings
    public class ExcelReaderSettings : FileReaderSettings
    {
        public string Worksheet { get; set; }
        public string Password { get; set; }
    }

    #endregion


    public class ExcelReader : TabularImportReader
    {
        #region Constructors
        private ExcelReader() : base()
        {
        }

        public ExcelReader(ExcelReaderSettings settings) : base()
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;
        }

        public ExcelReader(ExcelReaderSettings settings, Log log) : base(log)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;
        }

        #endregion

        #region Settings
        public ExcelReaderSettings Settings { get; private set; }

        #endregion

        #region Populate
        protected override DataGrid ReadRawData(DataGridColumns columns)
        {
            if (columns == null) throw new ArgumentNullException("columns");

            Framework.Excel.Interop.Excel excel = null;
            ExcelInterop.Worksheet worksheet = null;

            try
            {
                Log.Info(String.Format("Reading data from Excel File '{0}'...", Settings.FilePath));
                excel = Framework.Excel.Interop.Excel.Open(Settings.FilePath, Settings.Password);
                worksheet = !Settings.Worksheet.IsNullOrBlank() ? excel.Worksheet(Settings.Worksheet) : excel.Worksheet(0);
               
                ExcelInterop.Range range = worksheet.UsedRange;
                object[,] cells = range.Value2;
                
                DataGrid dataGrid = new DataGrid();

                /*
                 *  Headers
                 */

                var allColumns = columns.Expand();
                int columnCount = range.Columns.Count;
                for (int colIndex = 1; colIndex <= columnCount; colIndex++)
                {
                    string columnName = Convert.ToString(cells[1, colIndex]);
                    if (columnName.IsNullOrBlank() || (columns.Any() && !allColumns.Any(c => c.ColumnName.Equals(columnName))))
                    { continue; }
                    DataGridColumn column = new DataGridColumn(columnName) { OriginalSourceIndex = colIndex };
                    dataGrid.Columns.Add(column);                                        
                }

                /*
                 *  Data
                 */

                int rowCount;
                if (Settings.RowCount > 0)
                {
                    rowCount = (Settings.RowCount + 1 <= range.Rows.Count) ? Settings.RowCount + 1 : range.Rows.Count;
                }
                else if (Settings.RowCount < 0)
                {
                    rowCount = range.Rows.Count + Settings.RowCount;
                }
                else
                {
                    rowCount = range.Rows.Count;
                }

                for (int rowIndex = 2; rowIndex <= rowCount; rowIndex++)
                {
                    DataGridRow dataGridRow = dataGrid.Rows.New();
                    foreach (DataGridColumn column in dataGrid.Columns)
                    {
                        try
                        {
                            DataGridCell cell = dataGridRow.Cells.New();
                            cell.OriginalValue = cells[rowIndex, column.OriginalSourceIndex];
                        }
                        catch (Exception exception)
                        {
                            throw new Exception(String.Format("An error occurred while reading cell at Row '{0}', Column '{1}'."
                                , rowIndex
                                , column.OriginalSourceIndex)
                                , exception);
                        }
                    }
                }

                return dataGrid;
            }
            catch (ExcelInteropException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new ExcelInteropException(String.Format("An error occurred while Reading data from Excel file '{0}'.", Settings.FilePath), e);
            }
            finally
            {
                // Worksheet
                if (worksheet != null)
                {
                    Marshal.FinalReleaseComObject(worksheet);
                }
                // Excel
                if (excel != null)
                { excel.Dispose(); }
            }
        }

        #endregion
    }
}
