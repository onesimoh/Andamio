using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Xml.Linq;

using Andamio;
using Andamio.Collections;

namespace Andamio.Data.Flow.Formatters
{
    #region Collection
    [DebuggerDisplay("Count = {Count}")]
    public class GridColumnCollection : CollectionBase<GridColumn>
    {
        #region Constructors
        public GridColumnCollection() : base()
        {
        }

        public GridColumnCollection(IEnumerable<GridColumn> columns)
            : base(columns)
        {
        }
        #endregion

        #region Indexer
        public GridColumn this[string columnName]
        {
            get
            {
                var columns = this.Where(match => match.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                if (columns.Count() > 1)
                { throw new Exception(String.Format("Column '{0}' has been defined more than once.", columnName)); }
                return columns.FirstOrDefault();
            }
        }

        #endregion        

        #region Items
        public GridColumn New()
        {
            GridColumn column = new GridColumn();
            this.Add(column);
            return column;
        }

        #endregion
    }

    #endregion


    [DebuggerDisplay("{ColumnName}")]
    public class GridColumn : ColumnDescriptor
    {
        #region Constructors
        internal GridColumn() : base()
        {
            Initialize();
        }

        internal GridColumn(string name) : base(name)
        {
            Initialize();
        }

        private void Initialize()
        {
            AppendedColumns = new GridColumnCollection();
            AppendedColumns.ItemsInserted += OnAppendedColumnsInserted;
        }

        #endregion

        #region Conversion
        public static implicit operator DataGridColumn(GridColumn column)
        {
            DataGridColumn dataGridColumn = new DataGridColumn(column);
            column.AppendedColumns.ForEach(c => dataGridColumn.AppendedColumns.Add(c));
            return dataGridColumn;
        }

        #endregion

        #region Type
        public GridColumn OfType<T>()
        {
            ColumnType = typeof(T);
            return this;
        }

        public GridColumn OfType(Type columnType)
        {
            ColumnType = columnType;
            return this;
        }

        #endregion

        #region Size
        public GridColumn Size(int size)
        {
            if (size < 1) throw new ArgumentOutOfRangeException("size");
            ColumnSize = size;
            return this;
        }

        #endregion

        #region Position
        public GridColumn Position(int position)
        {
            if (position < 0) throw new ArgumentOutOfRangeException("position");
            ColumnPosition = position;
            return this;
        }

        public GridColumn Position(int position, int size)
        {
            Position(position);
            Size(size);
            return this;
        }

        #endregion

        #region Nullable
        public GridColumn Null()
        {
            AllowNull = true;
            return this;
        }

        public GridColumn Null(bool allowNull)
        {
            AllowNull = allowNull;
            return this;
        }

        #endregion

        #region Bind
        public GridColumn Bind(string name)
        {
            if (name.IsNullOrBlank()) throw new ArgumentNullException("name");
            BindingName = name;
            return this;
        }

        #endregion

        #region Format
        public GridColumn Format(string formatString)
        {
            DisplayFormat = formatString;
            return this;
        }

        public GridColumn Parse(string formatString)
        {
            ParseFormat = formatString;
            return this;
        }

        #endregion

        #region Value
        public GridColumn Value(int value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(decimal value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(float value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(double value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(string value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(Guid value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(DateTime value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(Func<int> value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(Func<decimal> value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(Func<float> value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(Func<double> value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(Func<string> value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(Func<DateTime> value)
        {
            base.CellValue = value;
            return this;
        }

        public GridColumn Value(Func<Guid> value)
        {
            base.CellValue = value;
            return this;
        }

        #endregion

        #region Column
        public GridColumn Column(string columnName)
        {
            GridColumn columnConfig = new GridColumn(columnName);
            Parent.Columns.Add(columnConfig);
            return columnConfig;
        }

        #endregion

        #region Appended
        public GridColumnCollection AppendedColumns { get; private set; }
        public GridColumn Append(string columnName)
        {
            GridColumn columnConfig = new GridColumn(columnName) { Parent = this.Parent };
            AppendedColumns.Add(columnConfig);
            return columnConfig;
        }

        void OnAppendedColumnsInserted(object sender, ItemEventArgs<IEnumerable<GridColumn>> e)
        {
            var appendedColumns = e.Item;
            if (appendedColumns != null && appendedColumns.Any())
            { appendedColumns.ForEach(c => c.Parent = this.Parent); }
        }

        #endregion

        #region Import
        public DataGrid Import()
        {
            if (Parent == null) throw new InvalidOperationException();
            return Parent.Import();
        }

        #endregion

        #region Parent
        internal GridFormatter Parent { get; set; }

        #endregion

        #region Serialization
        public override void FromElement(XElement element)
        {
            base.FromElement(element);

            // Appended Columns
            XNamespace xmlns = element.Name.Namespace;
            element.Elements(xmlns + "Append").ForEach(delegate(XElement columnNode)
            {
                GridColumn column = AppendedColumns.New();
                column.FromElement(columnNode);
            });
        }

        #endregion
    }
}
