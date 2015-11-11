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
    public class DataGridRows : CollectionBase<DataGridRow>
    {
        #region Constructors
        public DataGridRows()
            : base()
        {
        }

        public DataGridRows(IEnumerable<DataGridRow> rows)
            : base(rows)
        {
        }

        #endregion

        #region Items
        public DataGridRow New()
        {
            DataGridRow row = new DataGridRow();
            this.Add(row);
            return row;
        }

        #endregion
    }

    #endregion


    public class DataGridRow
    {
        #region Constructors
        internal DataGridRow()
        {
            Cells = new DataGridCells();
            Cells.ItemsInserted += OnCellsInserted;
        }

        #endregion

        #region Cells
        public DataGridCells Cells { get; private set; }
        void OnCellsInserted(object sender, ItemEventArgs<IEnumerable<DataGridCell>> e)
        {
            var cells = e.Item;
            if (cells != null)
            {
                cells.ForEach(delegate(DataGridCell cell) 
                {
                    cell.DataGrid = DataGrid;
                    cell.Row = this;
                });
            }
        }

        public void Empty()
        {
            Cells.Clear();
        }

        public DataGridCell this[string columnName]
        {
            get
            {
                DataGridColumn column = DataGrid.Columns[columnName];
                if (column == null) throw new Exception(String.Format("Invalid Column Name: '{0}'.", columnName));
                return Cells[column.Index()];
            }
        }

        public DataGridCell this[int index]
        {
            get
            {
                return Cells[index];
            }
        }

        #endregion

        #region Navigation
        public DataGridRow Next()
        {
            int rowIndex = Index();
            if ((rowIndex >= 0) && (rowIndex < DataGrid.Rows.Count - 1))
            { return DataGrid.Rows[++rowIndex]; }
            return null;
        }

        public DataGridRow Previous()
        {
            int rowIndex = Index();
            if (rowIndex > 0)
            { return DataGrid.Rows[--rowIndex]; }
            return null;
        }

        public int Index()
        {
            return (DataGrid != null) ? DataGrid.Rows.IndexOf(this) : -1;
        }

        public static DataGridRow operator ++(DataGridRow dataGridRow)
        {
            return dataGridRow.Next();
        }

        public static DataGridRow operator --(DataGridRow dataGridRow)
        {
            return dataGridRow.Previous();
        }

        #endregion

        #region DataGrid
        private DataGrid _dataGrid;
        public DataGrid DataGrid 
        {
            get { return _dataGrid; }
            internal set 
            { 
                _dataGrid = value;
                if (_dataGrid == null) throw new InvalidOperationException();
                Cells.ForEach(delegate(DataGridCell cell) 
                {
                    cell.DataGrid = _dataGrid;
                    cell.Row = this;
                });
            }
        }

        #endregion
    }
}
