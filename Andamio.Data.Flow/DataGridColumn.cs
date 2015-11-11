using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Linq;

using Andamio;
using Andamio.Collections;
using Andamio.Data.Flow.Formatters;

namespace Andamio.Data
{
    #region Collection
    [DebuggerDisplay("Count = {Count}")]
    public class DataGridColumns : CollectionBase<DataGridColumn>
    {
        #region Constructors
        public DataGridColumns() : base()
        {
        }

        public DataGridColumns(IEnumerable<DataGridColumn> columns)
            : base(columns)
        {
        }

        #endregion

        #region Indexer
        public DataGridColumn this[string columnName]
        {
            get
            {
                var columns = this.Where(match => match.ColumnName.Equals(columnName));
                if (columns.Count() > 1)
                { throw new ApplicationException(String.Format("Column '{0}' has been defined more than once.", columnName)); }
                return columns.FirstOrDefault();
            }
        }

        #endregion

        #region Items
        public DataGridColumn Add(string columnName, Type columnType)
        {
            DataGridColumn column = new DataGridColumn(columnName, columnType);
            Add(column);
            return column;
        }

        public DataGridColumn Add(string columnName)
        {
            return Add(columnName, typeof(String));
        }

        public DataGridColumn Add<T>(string columnName) where T : struct
        {
            return Add(columnName, typeof(T));
        }

        public IEnumerable<DataGridColumn> Find(string columnName)
        {
            return this.Where(match => match.ColumnName.Equals(columnName));
        }

        public IEnumerable<DataGridColumn> Find(string columnName, StringComparison comparison)
        {
            return this.Where(match => match.ColumnName.Equals(columnName, comparison));
        }

        public DataGridColumn New()
        {
            DataGridColumn column = new DataGridColumn();
            this.Add(column);
            return column;
        }

        public IEnumerable<DataGridColumn> Expand()
        {
            DataGridColumns columns = new DataGridColumns();
            foreach (DataGridColumn column in this)
            {
                columns.Add(column);
                if (column.AppendedColumns.Any())
                {
                    columns.AddRange(column.AppendedColumns.Expand());
                }
            }

            return columns;
        }

        #endregion
    }

    #endregion


    [DebuggerDisplay("{ColumnName}")]
    public class DataGridColumn : ColumnDescriptor
    {
        #region Constructors
        public DataGridColumn() : base()
        {
            Initialize();
        }

        public DataGridColumn(string columnName) : base(columnName)
        {
            Initialize();
        }

        public DataGridColumn(string columnName, Type columnType) : base(columnName)
        {
            if (columnType == null) throw new ArgumentNullException("columnType");
            ColumnType = columnType;
            Initialize();
        }

        public DataGridColumn(ColumnDescriptor columnDescriptor) : base(columnDescriptor.ColumnName)
        {
            if (columnDescriptor == null) throw new ArgumentNullException("columnDescriptor");
            ColumnName = columnDescriptor.ColumnName;
            ColumnType = columnDescriptor.ColumnType;
            ColumnSize = columnDescriptor.ColumnSize;
            ColumnPosition = columnDescriptor.ColumnPosition; 
            AllowNull = columnDescriptor.AllowNull;            
            DisplayFormat = columnDescriptor.DisplayFormat;
            ParseFormat = columnDescriptor.ParseFormat;
            BindingName = columnDescriptor.BindingName;
            BindingExpression = columnDescriptor.BindingExpression;
            BindingType = columnDescriptor.BindingType;
            CellValue = columnDescriptor.CellValue;
            ColumnMap = columnDescriptor.ColumnMap;

            Initialize();
        }

        private void Initialize()
        {
            OriginalSourceIndex = -1;
            AppendedColumns = new DataGridColumns();
        }

        #endregion

        #region Cell
        public DataGridCell Cell(object cellValue)
        {
            if (!AllowNull && cellValue == null)
            { throw new NullColumnException(String.Format("Column '{0}' does not accept Null values.", this)); }

            DataGridCell cell;
            try
            {
                switch (Type.GetTypeCode(BindingType))
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.DateTime:
                        cell = DataGridCell.ForType(BindingType, DisplayFormat);
                        cell.OriginalValue = cellValue.ToType(BindingType, ParseFormat);
                        return cell;
                    case TypeCode.String:
                        cell = new DataGridCell() { FormatString = DisplayFormat };
                        cellValue = cellValue.ToType(ColumnType);                        
                        if (cellValue != null)
                        { cell.OriginalValue = cellValue.ToString().Trim(); }
                        return cell;
                    case TypeCode.Object:
                        if (BindingType.Equals(typeof(Guid)))
                        {
                            cell = DataGridCell.ForType<Guid>(DisplayFormat);
                            cell.OriginalValue = cellValue.ToType<Guid>(ParseFormat);
                            return cell;
                        }
                        else if (BindingType.Equals(typeof(OleDateTime)))
                        {
                            cell = DataGridCell.ForType<OleDateTime>(DisplayFormat);
                            cell.OriginalValue = cellValue.ToType<OleDateTime>(ParseFormat);
                            return cell;
                        }
                        else if (BindingType.IsNullableType())
                        {
                            Type underlyingType = Nullable.GetUnderlyingType(BindingType);
                            cell = DataGridCell.ForNullType(underlyingType, DisplayFormat);
                            cell.OriginalValue = cellValue.ToType(underlyingType);
                            return cell;
                        }
                        else
                        {
                            throw new ColumnTypeException(String.Format("Column Type '{0}' Is Not Supported.", BindingType));
                        }
                    default:
                        return new DataGridCell(cellValue);
                }
            }
            catch (Exception exception)
            {
                throw new CellValueCastException(String.Format("Value '{0}' cannot be converted to Type '{1}'.", cellValue, BindingType)
                    , exception);  
            }
        }

        public DataGridCell Cell()
        {
            switch (Type.GetTypeCode(BindingType))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.DateTime:
                    return DataGridCell.ForType(BindingType, DisplayFormat);
                case TypeCode.String:
                    return new DataGridCell() { FormatString = DisplayFormat };                
                case TypeCode.Object:
                    if (BindingType.Equals(typeof(Guid)))
                    {
                        return DataGridCell.ForType<Guid>(DisplayFormat);
                    }
                    else if (BindingType.Equals(typeof(OleDateTime)))
                    {
                        return DataGridCell.ForType<OleDateTime>(DisplayFormat);
                    }
                    else if (BindingType.IsNullableType())
                    {
                        Type underlyingType = Nullable.GetUnderlyingType(BindingType);
                        return DataGridCell.ForNullType(underlyingType, DisplayFormat);
                    }
                    else
                    {
                        throw new ColumnTypeException(String.Format("Type '{0}' Is Not Supported.", BindingType));
                    }
                default:
                    return new DataGridCell() { FormatString = DisplayFormat };
            }
        }

        #endregion

        #region Navigation
        public DataGridColumn Next()
        {
            int colndex = Index();
            if ((colndex >= 0) && (colndex < DataGrid.Columns.Count - 1))
            { return DataGrid.Columns[++colndex]; }
            return null;
        }

        public DataGridColumn Previous()
        {
            int colndex = Index();
            if (colndex > 0)
            { return DataGrid.Columns[--colndex]; }
            return null;
        }

        public int Index()
        {
            return (DataGrid != null) ? DataGrid.Columns.IndexOf(this) : -1;
        }

        public string Coordinates()
        {
            if (ColumnPosition > 0)
            {
                return String.Format("{0}, {1}", ColumnPosition, ColumnSize);
            }
            else if (OriginalSourceIndex >= 0)
            {
                return OriginalSourceIndex.ToString();
            }
            else
            {
                return Index().ToString();
            }
        }

        public IEnumerable<DataGridCell> Cells()
        {
            if (DataGrid == null)
            { yield return null; }

            int columnIndex = Index();
            foreach (DataGridRow row in DataGrid.Rows)
            {
                yield return row[columnIndex];
            }                        
        }

        public static DataGridColumn operator ++(DataGridColumn dataGridColumn)
        {
            return dataGridColumn.Next();
        }

        public static DataGridColumn operator --(DataGridColumn dataGridColumn)
        {
            return dataGridColumn.Previous();
        }

        #endregion

        #region DataGrid
        public DataGrid DataGrid { get; internal set; }

        #endregion 

        #region RawIndex
        internal int OriginalSourceIndex { get; set; }

        #endregion

        #region Appended Columns
        public DataGridColumns AppendedColumns { get; private set; }

        public bool HasMappedColumns
        {
            get
            {
                if (!ColumnMap.IsNullOrBlank())
                {
                    return true;
                }

                if (AppendedColumns.Expand().Any(match => !ColumnMap.IsNullOrBlank()))
                {
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return ColumnName;
        }

        #endregion
    }
}
