using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

using Andamio;
using Andamio.Data;
using Andamio.Diagnostics;
using Andamio.Data.Flow.Media.Readers;

namespace Andamio.Data.Flow.Media.Readers
{
    public class FixedLengthReader : ImportReader
    {
        #region Constructors
        private FixedLengthReader() : base()
        {
        }

        public FixedLengthReader(FileReaderSettings settings) : base()
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;
        }

        public FixedLengthReader(FileReaderSettings settings, Log log) : base(log)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;
        }

        #endregion

        #region Settings
        public FileReaderSettings Settings { get; protected set; }

        #endregion

        #region Populate
        protected override DataGrid ReadRawData(DataGridColumns columns)
        {
            if (columns == null) throw new ArgumentNullException("columns");
            if (!columns.Any()) throw new ArgumentException("No columns were specified.", "columns");

            Log.Info(String.Format("Reading data from Fixed-Length File '{0}'...", Settings.FilePath));
            using (StreamReader reader = new StreamReader(Settings.FilePath))
            {
                if (reader.EndOfStream)
                { throw new AbortException(String.Format("File '{0}' is Empty.", Settings.FilePath)); }

                DataGrid dataGrid = new DataGrid();

                /*
                *  Headers
                */

                var allColumns = columns.Expand().Select(c => new DataGridColumn(c.ColumnName));
                dataGrid.Columns.AddRange(allColumns);

                /*
                *  Data
                */

                while (!reader.EndOfStream)
                {
                    DataGridRow dataGridRow = dataGrid.Rows.New();

                    string dataLine = reader.ReadLine();
                    foreach (DataGridColumn column in dataGrid.Columns)
                    {
                        try
                        {
                            string cellValue = dataLine.Substring(column.ColumnPosition - 1, column.ColumnSize);
                            dataGridRow.Cells.Add(DataGridCell.StringCell(cellValue));
                        }
                        catch (Exception exception)
                        {
                            throw new Exception(String.Format("An error occurred while reading cell at Row: '{0}', Position: '{1}, {2}'."
                                , dataGridRow.Index() + 1
                                , column.ColumnPosition
                                , column.ColumnSize)
                                , exception);
                        }
                    }
                }

                return dataGrid;
            }
        }

        protected override void ProcessHeaders(DataGridColumns columns, DataGrid rawData)
        {
            if (rawData == null) throw new ArgumentNullException("rawData");
            if (columns == null) throw new ArgumentNullException("columns");
            if (!columns.Any()) throw new ArgumentException("No columns were specified.", "columns");

            foreach (DataGridColumn dataGridColumn in columns.Expand())
            {
                if (dataGridColumn.CellValue == null)
                {
                    Log.Info(String.Format("Column '{0}' of Type '{1}' at column position '{2}, {3}'."
                        , dataGridColumn
                        , dataGridColumn.ColumnType.FriendlyName()
                        , dataGridColumn.ColumnPosition
                        , dataGridColumn.ColumnSize));
                }
                else
                {
                    Log.Info(String.Format("A cell column value was provided for Column '{0}' of Type '{1}'. Cells belonging to this column will inherit this value."
                        , dataGridColumn
                        , dataGridColumn.ColumnType.FriendlyName()));
                }

                if (!dataGridColumn.ColumnType.DerivesFromType(dataGridColumn.BindingType))
                {
                    Log.Warning(String.Format("Column '{0}': Type mismatch between Column Type '{1}' and Binding Type {2}. Data may be lost during Type Casting."
                        , dataGridColumn
                        , dataGridColumn.ColumnType.FriendlyName()
                        , dataGridColumn.BindingType));
                }
            }
        }

        #endregion
    }
}
