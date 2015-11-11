using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

using Andamio;
using Andamio.Collections;
using Andamio.Diagnostics;

namespace Andamio.Data.Flow.Media.Readers
{
    #region Settings
    public class ReaderSettings
    {
        public int RowCount { get; set; }
        public int SkipCount { get; set; }
    }

    public class FileReaderSettings : ReaderSettings
    {
        public string FilePath { get; set; }
    }

    #endregion


    public abstract class ImportReader : Disposable
    {
        #region Constructors
        protected ImportReader() : base()
        {
            Log = new Log();
            Log.AttachDebugger();
        }

        protected ImportReader(Log log) : this()
        {
            if (log == null) throw new ArgumentNullException("log");
            Log = log;
        }

        #endregion

        #region Log
        public Log Log { get; set; }

        #endregion

        #region Populate
        protected abstract DataGrid ReadRawData(DataGridColumns columns);
        protected abstract void ProcessHeaders(DataGridColumns columns, DataGrid rawData);
        public virtual void Populate(DataGrid dataGrid)
        {
            if (dataGrid == null) throw new ArgumentNullException("dataGrid");

            try
            {
                Log.Info("Beginning Data Import...");
                DataGrid rawData = ReadRawData(dataGrid.Columns);
                if (!rawData.Rows.Any())
                {
                    throw new AbortException("Specified Data Source is empty.");
                }
                Log.Info(String.Format("Data successfully read from file. Total Rows: {0}.", rawData.Rows.Count()));

                /*
                *  Headers
                */

                Log.Info("Processing headers from imported data...");
                ProcessHeaders(dataGrid.Columns, rawData);
                Log.Info("Headers successfully processed.");

                /*
                *  Data
                */

                dataGrid.Empty();
                Log.Info("Processing imported data...");
                foreach (DataGridRow rawDataRow in rawData.Rows)
                {
                    Log.Trace(String.Format("Processing imported row '{0}'.", rawDataRow.Index()));

                    // Pass 1: Process Raw Data from Source
                    DataGridRow dataGridRow = dataGrid.Rows.New();
                    foreach (DataGridColumn dataColumn in dataGrid.Columns)
                    {
                        try
                        {
                            object cellValue = GetValue(dataColumn, rawDataRow);
                            DataGridCell cell = (cellValue != null) ? dataColumn.Cell(cellValue) : dataColumn.Cell();
                            dataGridRow.Cells.Add(cell);
                        }
                        catch (Exception exception)
                        {
                            throw new AbortException(String.Format("An error occurred while reading cell at Row: '{0}', Column: '{1}'."
                                , dataGridRow.Index() + 1
                                , dataColumn)
                                , exception);
                        }
                    }

                    // Pass 2: Process Mapped Columns
                    foreach (DataGridColumn mappedColumn in dataGrid.Columns.Where(match => match.HasMappedColumns))
                    {
                        try
                        {
                            DataGridCell cell = dataGridRow[mappedColumn.ColumnName];
                            object cellValue = GetMappedValue(mappedColumn, dataGridRow);
                            cell.OriginalValue = cellValue;
                        }
                        catch (Exception exception)
                        {
                            throw new AbortException(String.Format("An error occurred while reading cell at Row: '{0}', Column: '{1}'."
                                , dataGridRow.Index() + 1
                                , mappedColumn)
                                , exception);
                        }
                    }
                }

                Log.Info(String.Format("Imported data successfully processed. Total Rows processed: {0}", dataGrid.Rows.Count()));
            }
            catch (AbortException exception)
            {
                Log.Critrical("Data Import Failed!", exception);
                throw exception;
            }
            catch (Exception exception)
            {
                Log.Error("An unexpected Error occurred…", exception);
                Log.Critrical("Data Import Failed!");
                throw new AbortException(exception.Message, exception);
            }
        }

        private object GetValue(DataGridColumn dataColumn, DataGridRow dataRow)
        {
            if (dataRow == null) throw new ArgumentNullException("dataRow");
            if (dataColumn == null) throw new ArgumentNullException("dataColumn");

            object cellValue;
            if (dataColumn.CellValue != null)
            {
                cellValue = dataColumn.CellValue.GetType().DerivesFromType(typeof(Func<>))
                    ? ((Delegate)dataColumn.CellValue).DynamicInvoke()
                    : dataColumn.CellValue;
            }
            else if (dataColumn.ColumnMap.IsNullOrBlank())
            {
                DataGridCell cell = dataRow.Cells.Where(match => match.Column.ColumnName.Equals(dataColumn.ColumnName)).FirstOrDefault();
                if (cell != null)
                {
                    if (cell.OriginalValue != null && cell.OriginalValue is String)
                    {
                        string stringValue = cell.OriginalValue.ToString().Trim();
                        cellValue = !stringValue.IsNullOrBlank() ? stringValue : null;
                    }
                    else
                    {
                        cellValue = cell.OriginalValue;
                    }
                }
                else
                {
                    cellValue = null;
                }
            }
            else
            {
                cellValue = null;
            }

            
            if (dataColumn.AppendedColumns.Any())
            {
                foreach (DataGridColumn appendedColumn in dataColumn.AppendedColumns)
                {
                    object appendedValue = GetValue(appendedColumn, dataRow);
                    string appendedValueText = (appendedValue != null) ? appendedValue.ToString() : String.Empty;

                    string cellValueText = (cellValue != null) ? cellValue.ToString() : String.Empty;
                    cellValue = cellValueText + appendedValueText;
                }
            }

            return cellValue;
        }

        private object GetMappedValue(DataGridColumn dataColumn, DataGridRow dataRow)
        {
            if (dataRow == null) throw new ArgumentNullException("dataRow");
            if (dataColumn == null) throw new ArgumentNullException("dataColumn");

            string columnName = !dataColumn.ColumnMap.IsNullOrBlank() ? dataColumn.ColumnMap : dataColumn.ColumnMap;
            DataGridCell cell = dataRow.Cells.Where(match => match.Column.ColumnName.Equals(dataColumn.ColumnMap)).FirstOrDefault();
            object cellValue = (cell != null) ? cell.OriginalValue : null;

            if (dataColumn.AppendedColumns.Any())
            {
                foreach (DataGridColumn appendedColumn in dataColumn.AppendedColumns)
                {
                    object appendedValue = GetMappedValue(appendedColumn, dataRow);
                    string appendedValueText = (appendedValue != null) ? appendedValue.ToString() : String.Empty;

                    string cellValueText = (cellValue != null) ? cellValue.ToString() : String.Empty;
                    cellValue = cellValueText + appendedValueText;
                }
            }

            return cellValue;
        }

        #endregion

        #region Disposable
        protected override void Cleanup()
        {
        }

        #endregion
    }


    public abstract class TabularImportReader : ImportReader
    {
        #region Constructors
        protected TabularImportReader() : base()
        {
        }

        protected TabularImportReader(Log log) : base(log)
        {
        }

        #endregion

        #region Populate
        protected override void ProcessHeaders(DataGridColumns columns, DataGrid rawData)
        {
            if (rawData == null) throw new ArgumentNullException("rawData");
            if (columns == null) throw new ArgumentNullException("columns");

            if (!columns.Any())
            {
                Log.Warning("No column mappings were provided. Columns will be derived from source.");
                columns.AddRange(rawData.Columns);
            }

            foreach (DataGridColumn dataGridColumn in columns.Expand())
            {
                if (dataGridColumn.CellValue != null)
                {
                    Log.Info(String.Format("A cell column value was provided for Column '{0}' of Type '{1}'. Cells belonging to this column will inherit this value."
                        , dataGridColumn
                        , dataGridColumn.ColumnType.FriendlyName()));
                }
                else if (!dataGridColumn.ColumnMap.IsNullOrBlank())
                {
                    Log.Warning(String.Format("Column '{0}' of Type '{1}' is mapped to Column '{2}' from which its value will be derived."
                        , dataGridColumn
                        , dataGridColumn.ColumnType.FriendlyName()
                        , dataGridColumn.ColumnMap));
                }
                else
                {
                    var allColumns = rawData.Columns.Find(dataGridColumn.ColumnName);
                    DataGridColumn column = allColumns.FirstOrDefault();
                    if (allColumns.Count() > 1)
                    {
                        Log.Warning(String.Format("Multiple instances of Column '{0}' of Type '{1}'. Resolving to first instance found at position '{2}'."
                            , dataGridColumn
                            , dataGridColumn.ColumnType.FriendlyName()
                            , column.OriginalSourceIndex));
                    }
                    else if (allColumns.Count() == 1)
                    {
                        Log.Info(String.Format("Column '{0}' of Type '{1}' found at column position '{2}'."
                            , dataGridColumn
                            , dataGridColumn.ColumnType.FriendlyName()
                            , column.OriginalSourceIndex));
                    }
                    else
                    {
                        if (!dataGridColumn.AppendedColumns.Any())
                        {
                            throw new AbortException(String.Format("Column '{0}' Not available and no Value was provided.", dataGridColumn));
                        }

                        Log.Warning(String.Format("Composite Column '{0}' of Type '{1}' has not been mapped but a value may still be derived from its appended columns."
                            , dataGridColumn
                            , dataGridColumn.ColumnType.FriendlyName()));
                    }
                }

                if (!dataGridColumn.ColumnType.DerivesFromType(dataGridColumn.BindingType))
                {
                    Log.Warning(String.Format("Column '{0}': Type mismatch between Column Type '{1}' and Binding Type '{2}'. Data may be lost during Type Casting."
                        , dataGridColumn
                        , dataGridColumn.ColumnType.FriendlyName()
                        , dataGridColumn.BindingType.FriendlyName()));
                }
            }
        }

        #endregion

        #region Disposable
        protected override void Cleanup()
        {
        }

        #endregion
    }
}
