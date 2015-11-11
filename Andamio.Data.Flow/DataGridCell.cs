using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Andamio;
using Andamio.Collections;

namespace Andamio.Data
{
    #region Collection
    [DebuggerDisplay("Count = {Count}")]
    public class DataGridCells : CollectionBase<DataGridCell>
    {
        #region Constructors
        public DataGridCells() : base()
        {
        }

        public DataGridCells(IEnumerable<DataGridCell> cells) : base(cells)
        {
        }

        #endregion

        #region Conversion
        public static implicit operator DataGridCells(List<DataGridCell> cells)
        {
            return new DataGridCells(cells);
        }

        public static implicit operator DataGridCells(DataGridCell[] cells)
        {
            return new DataGridCells(cells);
        }

        #endregion

        #region Items
        public DataGridCell AddCell(string value)
        {
            DataGridCell cell = DataGridCell.StringCell();
            cell.OriginalValue = value;
            Add(cell);
            return cell;
        }

        public DataGridCell<T> AddCell<T>(T value) where T : struct
        {
            DataGridCell<T> cell = DataGridCell.ForType<T>();
            cell.Value = value;
            Add(cell);
            return cell;
        }

        public DataGridCell New()
        {
            DataGridCell cell = new DataGridCell();
            Add(cell);
            return cell;
        }

        #endregion

        #region Find
        public IEnumerable<DataGridCell> Find(string cellValue)
        {
            return this.Where(match => match.OriginalValue.ToString().Equals(cellValue));
        }
        public IEnumerable<DataGridCell> Find(string cellValue, StringComparison comparison)
        {
            return this.Where(match => match.OriginalValue.ToString().Equals(cellValue, comparison));
        }

        #endregion
    }

    #endregion


    [DebuggerDisplay("{OriginalValue}")]
    public class DataGridCell
    {
        #region Constructors
        public DataGridCell()
        {
        }

        public DataGridCell(object originalValue)
        {
            OriginalValue = originalValue;
        }

        #endregion

        #region Factory
        public static DataGridCell StringCell()
        {
            return new DataGridCell();
        }

        public static DataGridCell StringCell(string value)
        {
            return new DataGridCell() { OriginalValue = value };
        }

        public static DataGridCell ForType(Type type, string formatString = null)
        {
            Type cellGenericType = typeof(DataGridCell<>).MakeGenericType(type);
            DataGridCell cell = (DataGridCell)Activator.CreateInstance(cellGenericType);
            cell.FormatString = formatString;
            return cell;
        }

        public static DataGridCell<T> ForType<T>(string formatString = null)
            where T : struct
        {
            return new DataGridCell<T>() { FormatString = formatString };
        }

        public static DataGridCell ForNullType(Type type, string formatString = null)
        {
            Type cellGenericType = typeof(NullableDataGridCell<>).MakeGenericType(type);
            DataGridCell cell = (DataGridCell)Activator.CreateInstance(cellGenericType);
            cell.FormatString = formatString;
            return cell;
        }


        #endregion

        #region Navigation
        public DataGrid DataGrid { get; internal set; }
        public DataGridRow Row { get; internal set; }
        public DataGridColumn Column
        {
            get
            {
                return (DataGrid != null) ? DataGrid.Columns[Index()] : null;
            }
        }
        
        public int Index()
        {
            return (Row != null) ? Row.Cells.IndexOf(this) : -1;
        }

        public DataGridCell Up()
        {            
            if (Row != null)
            {
                DataGridRow previousRow = Row.Previous();
                if (previousRow != null)
                {
                    int colIndex = Index();
                    return previousRow.Cells[colIndex];
                }
            }
            return null;
        }

        public DataGridCell Down()
        {
            if (Row != null)
            {
                DataGridRow nextRow = Row.Next();
                if (nextRow != null)
                {
                    int colIndex = Index();
                    return nextRow.Cells[colIndex];
                }
            }
            return null;
        }

        public DataGridCell Left()
        {
            int index = Index();
            if (index > 0)
            { return Row.Cells[--index]; }

            return null;
        }

        public DataGridCell Right()
        {
            int index = Index();
            if ((index >= 0) && (index < Row.Cells.Count - 1))
            { return Row.Cells[++index]; }

            return null;
        }

        public static DataGridCell operator ++(DataGridCell dataGridCell)
        {
            return dataGridCell.Right();
        }

        public static DataGridCell operator --(DataGridCell dataGridCell)
        {
            return dataGridCell.Left();
        }

        #endregion

        #region Value
        public object OriginalValue { get; set; }

        #endregion

        #region Format
        public string FormatString { get; set; }
        public virtual string Format()
        {
            if (OriginalValue == null) return String.Empty;
            if (!FormatString.IsNullOrBlank()) return Format(FormatString);
            return (Column != null)  ? Format(Column.DisplayFormat) : OriginalValue.ToString();
        }

        public virtual string Format(string formatString)
        {
            if (OriginalValue == null) return String.Empty;
            return !formatString.IsNullOrBlank() ? String.Format(formatString, OriginalValue) : OriginalValue.ToString();
        }

        #endregion
    }


    [DebuggerDisplay("{Value}")]
    public class DataGridCell<T> : DataGridCell
        where T : struct
    {
        #region Constructors
        public DataGridCell() : base()
        {
        }

        public DataGridCell(T cellValue) : base(cellValue)
        {
            Value = cellValue;
        }

        #endregion

        #region Value
        public T Value
        {
            get { return (T) OriginalValue; }
            set { OriginalValue = (T) value; }
        }

        #endregion

        #region Format
        public override string Format()
        {
            if (OriginalValue == null) return String.Empty;
            if (!FormatString.IsNullOrBlank()) return Format(FormatString);
            return (Column != null) ? Format(Column.DisplayFormat) : Value.ToString();
        }

        public override string Format(string formatString)
        {
            if (OriginalValue == null) return String.Empty;
            return !formatString.IsNullOrBlank() ? String.Format(formatString, Value) : Value.ToString();
        }

        #endregion
    }


    [DebuggerDisplay("{Value}")]
    public class NullableDataGridCell<T> : DataGridCell
        where T : struct
    {
        #region Constructors
        public NullableDataGridCell()
            : base()
        {
        }

        public NullableDataGridCell(T? cellValue)
            : base(cellValue)
        {
            Value = cellValue;
        }

        public NullableDataGridCell(T cellValue)
            : base(cellValue)
        {
            Value = cellValue;
        }

        #endregion

        #region Value
        public Nullable<T> Value
        {
            get { return (Nullable<T>) OriginalValue; }
            set { OriginalValue = (Nullable<T>) value; }
        }

        #endregion

        #region Format
        public override string Format()
        {
            if (!Value.HasValue) return String.Empty;
            if (!FormatString.IsNullOrBlank()) return Format(FormatString);
            return (Column != null) ? Format(Column.DisplayFormat) : ((T)Value).ToString();
        }

        public override string Format(string formatString)
        {
            if (!Value.HasValue) return String.Empty;
            return !formatString.IsNullOrBlank() ? String.Format(formatString, (T)Value) : ((T)Value).ToString();
        }

        #endregion
    }
}
