using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Xml.Linq;
using System.Data.SqlClient;

using Andamio;
using Andamio.Collections;

namespace Andamio.Data.Flow.Formatters
{
    #region Collection
    [DebuggerDisplay("Count = {Count}")]
    public class DatabaseColumnCollection : CollectionBase<DatabaseColumn>
    {
        #region Constructors
        public DatabaseColumnCollection()
            : base()
        {
        }

        public DatabaseColumnCollection(IEnumerable<DatabaseColumn> columns)
            : base(columns)
        {
        }
        #endregion

        #region Indexer
        public DatabaseColumn this[string columnName]
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
        public DatabaseColumn New()
        {
            DatabaseColumn column = new DatabaseColumn();
            this.Add(column);
            return column;
        }

        #endregion
    }

    #endregion


    [DebuggerDisplay("{ColumnName}")]
    public class DatabaseColumn : ColumnDescriptor
    {
        #region Constructors
        internal DatabaseColumn() : base()
        {
            Initialize();
        }

        internal DatabaseColumn(string name) : base(name)
        {
            Initialize();
        }

        private void Initialize()
        {
            AppendedColumns = new DatabaseColumnCollection();
            AppendedColumns.ItemsInserted += OnAppendedColumnsInserted;
        }

        #endregion

        #region Conversion
        public static implicit operator DataGridColumn(DatabaseColumn column)
        {
            DataGridColumn dataGridColumn = new DataGridColumn(column);
            column.AppendedColumns.ForEach(c => dataGridColumn.AppendedColumns.Add(c));
            return dataGridColumn;
        }

        #endregion

        #region Type
        public DatabaseColumn OfType<T>()
        {
            ColumnType = typeof(T);
            return this;
        }

        public DatabaseColumn OfType(Type columnType)
        {
            ColumnType = columnType;
            return this;
        }

        #endregion

        #region Size
        public DatabaseColumn Size(int size)
        {
            if (size < 1) throw new ArgumentOutOfRangeException("size");
            ColumnSize = size;
            return this;
        }

        #endregion

        #region Position
        public DatabaseColumn Position(int position)
        {
            if (position < 0) throw new ArgumentOutOfRangeException("position");
            ColumnPosition = position;
            return this;
        }

        public DatabaseColumn Position(int position, int size)
        {
            Position(position);
            Size(size);
            return this;
        }

        #endregion

        #region Nullable
        public DatabaseColumn Null()
        {
            AllowNull = true;
            return this;
        }

        public DatabaseColumn Null(bool allowNull)
        {
            AllowNull = allowNull;
            return this;
        }

        #endregion

        #region Bind
        public DatabaseColumn Bind(string name)
        {
            if (name.IsNullOrBlank()) throw new ArgumentNullException("name");
            BindingName = name;
            return this;
        }

        #endregion

        #region Format
        public DatabaseColumn Format(string format)
        {
            DisplayFormat = format;
            return this;
        }

        public DatabaseColumn Parse(string format)
        {
            ParseFormat = format;
            return this;
        }

        #endregion

        #region Value
        public DatabaseColumn Value(int value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(decimal value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(float value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(double value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(string value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(Guid value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(DateTime value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(Func<int> value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(Func<decimal> value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(Func<float> value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(Func<double> value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(Func<string> value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(Func<DateTime> value)
        {
            base.CellValue = value;
            return this;
        }

        public DatabaseColumn Value(Func<Guid> value)
        {
            base.CellValue = value;
            return this;
        }

        #endregion

        #region Column
        public DatabaseColumn Column(string columnName)
        {
            DatabaseColumn columnConfig = new DatabaseColumn(columnName);
            Parent.Columns.Add(columnConfig);
            return columnConfig;
        }

        #endregion

        #region Appended
        public DatabaseColumnCollection AppendedColumns { get; private set; }
        public DatabaseColumn Append(string columnName)
        {
            DatabaseColumn columnConfig = new DatabaseColumn(columnName) { Parent = this.Parent };
            AppendedColumns.Add(columnConfig);
            return columnConfig;
        }

        void OnAppendedColumnsInserted(object sender, ItemEventArgs<IEnumerable<DatabaseColumn>> e)
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
        internal DatabaseFormatter Parent { get; set; }

        #endregion

        #region Sql
        public string DbColumnName()
        {
            return "[" + BindingName + "]"; 
        }

        public string DbParamName()
        {
            return "@" + BindingName;
        }

        public SqlParameter DbParam(DataGridCell cell)
        {
            if (cell == null) throw new ArgumentNullException("cell");
            return DbParam(cell.OriginalValue);
        }

        public SqlParameter DbParam(object value)
        {            
            SqlParameter sqlParam;
            if (value == null)
            {
                sqlParam = new SqlParameter(DbParamName(), DBNull.Value);
            }
            else if (value is OleDateTime) 
            {
                sqlParam = new SqlParameter(DbParamName(), ((OleDateTime) value).Value);
            }
            else
            {
                sqlParam = new SqlParameter(DbParamName(), value);
            }
            
            sqlParam.IsNullable = AllowNull;
            return sqlParam;
        }

        #endregion

        #region Serialization
        public override void FromElement(XElement element)
        {
            base.FromElement(element);

            // Appended Columns
            XNamespace xmlns = element.Name.Namespace;
            element.Elements(xmlns + "Append").ForEach(delegate(XElement columnNode)
            {
                DatabaseColumn column = AppendedColumns.New();
                column.FromElement(columnNode);
            });
        }

        #endregion
    }
}
