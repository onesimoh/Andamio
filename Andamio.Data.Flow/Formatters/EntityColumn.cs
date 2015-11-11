using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Configuration;
using System.Xml.Linq;

using Andamio;
using Andamio.Collections;

namespace Andamio.Data.Flow.Formatters
{
    #region Collection
    [DebuggerDisplay("Count = {Count}")]
    public class EntityColumnCollection<EntityT> : CollectionBase<EntityColumn<EntityT>>
    {
        #region Constructors
        public EntityColumnCollection()
            : base()
        {
        }

        public EntityColumnCollection(IEnumerable<EntityColumn<EntityT>> columns)
            : base(columns)
        {
        }
        #endregion

        #region Indexer
        public EntityColumn<EntityT> this[string columnName]
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
        public EntityColumn<EntityT> New()
        {
            EntityColumn<EntityT> column = new EntityColumn<EntityT>();
            this.Add(column);
            return column;
        }

        #endregion
    }

    #endregion


    [DebuggerDisplay("{ColumnName}")]
    public class EntityColumn<EntityT> : ColumnDescriptor
    {
        #region Constructors
        internal EntityColumn() : base()
        {
            Initialize();
        }

        internal EntityColumn(string name) : base(name)
        {
            Initialize();
        }

        private void Initialize()
        {
            AppendedColumns = new EntityColumnCollection<EntityT>();
            AppendedColumns.ItemsInserted += OnAppendedColumnsInserted;
        }

        #endregion

        #region Conversion
        public static implicit operator DataGridColumn(EntityColumn<EntityT> column)
        {
            DataGridColumn dataGridColumn = new DataGridColumn(column);
            column.AppendedColumns.ForEach(c => dataGridColumn.AppendedColumns.Add(c));
            return dataGridColumn;
        }

        #endregion

        #region Type
        public EntityColumn<EntityT> OfType<T>()
        {
            ColumnType = typeof(T);
            return this;
        }

        public EntityColumn<EntityT> OfType(Type columnType)
        {
            ColumnType = columnType;
            return this;
        }

        #endregion

        #region Size
        public EntityColumn<EntityT> Size(int size)
        {
            if (size < 1) throw new ArgumentOutOfRangeException("size");
            ColumnSize = size;
            return this;
        }

        #endregion

        #region Position
        public EntityColumn<EntityT> Position(int position)
        {
            if (position < 0) throw new ArgumentOutOfRangeException("position");
            ColumnPosition = position;
            return this;
        }

        public EntityColumn<EntityT> Position(int position, int size)
        {
            if (position < 0) throw new ArgumentOutOfRangeException("position");
            if (size < 1) throw new ArgumentOutOfRangeException("size");
            ColumnPosition = position;
            ColumnSize = size;
            return this;
        }

        #endregion

        #region Nullable
        public EntityColumn<EntityT> Null()
        {
            AllowNull = true;
            return this;
        }

        public EntityColumn<EntityT> NotNull()
        {
            AllowNull = false;
            return this;
        }

        #endregion

        #region Bind
        public EntityColumn<EntityT> Bind(string name)
        {
            if (name.IsNullOrBlank()) throw new ArgumentNullException("name");
            BindingName = name;
            return this;
        }

        public EntityColumn<EntityT> Bind(Expression<Func<EntityT, string>> bindingExpression)
        {
            if (bindingExpression == null) throw new ArgumentNullException("propertyExpression");
            BindingType = typeof(string);
            BindingExpression = bindingExpression;
            return Bind(((MemberExpression)bindingExpression.Body).Member.Name);
        }

        public EntityColumn<EntityT> Bind(Expression<Func<EntityT, decimal>> bindingExpression)
        {
            return BindProperty(bindingExpression);
        }
        public EntityColumn<EntityT> Bind(Expression<Func<EntityT, decimal?>> bindingExpression)
        {
            return BindNullableProperty(bindingExpression);
        }

        public EntityColumn<EntityT> Bind(Expression<Func<EntityT, int>> bindingExpression)
        {
            return BindProperty(bindingExpression);
        }
        public EntityColumn<EntityT> Bind(Expression<Func<EntityT, int?>> bindingExpression)
        {
            return BindNullableProperty(bindingExpression);
        }

        public EntityColumn<EntityT> Bind(Expression<Func<EntityT, float>> bindingExpression)
        {
            return BindProperty(bindingExpression);
        }
        public EntityColumn<EntityT> Bind(Expression<Func<EntityT, float?>> bindingExpression)
        {
            return BindNullableProperty(bindingExpression);
        }

        public EntityColumn<EntityT> Bind(Expression<Func<EntityT, Guid>> bindingExpression)
        {
            return BindProperty(bindingExpression);
        }
        public EntityColumn<EntityT> Bind(Expression<Func<EntityT, Guid?>> bindingExpression)
        {
            return BindNullableProperty(bindingExpression);
        }

        public EntityColumn<EntityT> Bind<T>(Expression<Func<EntityT, Nullable<T>>> bindingExpression)
            where T : struct
        {
            return BindNullableProperty(bindingExpression);
        }
        public EntityColumn<EntityT> Bind<T>(Expression<Func<EntityT, T>> bindingExpression)
            where T : struct
        {
            return BindProperty(bindingExpression);
        }

        public EntityColumn<EntityT> BindNullableProperty<T>(Expression<Func<EntityT, Nullable<T>>> bindingExpression)
            where T : struct
        {
            if (bindingExpression == null) throw new ArgumentNullException("propertyExpression");
            BindingType = typeof(Nullable<T>);
            BindingExpression = bindingExpression;
            return Bind(((MemberExpression)bindingExpression.Body).Member.Name);
        }
        public EntityColumn<EntityT> BindProperty<T>(Expression<Func<EntityT, T>> bindingExpression)
            where T : struct
        {
            if (bindingExpression == null) throw new ArgumentNullException("propertyExpression");
            BindingType = typeof(T);
            BindingExpression = bindingExpression;
            return Bind(((MemberExpression)bindingExpression.Body).Member.Name);
        }

        #endregion

        #region Format
        public EntityColumn<EntityT> Format(string formatString)
        {
            DisplayFormat = formatString;
            return this;
        }

        public EntityColumn<EntityT> Parse(string formatString)
        {
            ParseFormat = formatString;
            return this;
        }

        #endregion

        #region Value
        public EntityColumn<EntityT> Value(int value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(decimal value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(float value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(double value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(string value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(Guid value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(DateTime value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(Func<int> value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(Func<decimal> value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(Func<float> value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(Func<double> value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(Func<string> value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(Func<DateTime> value)
        {
            base.CellValue = value;
            return this;
        }

        public EntityColumn<EntityT> Value(Func<Guid> value)
        {
            base.CellValue = value;
            return this;
        }

        #endregion

        #region Column
        public EntityColumn<EntityT> Column(string columnName)
        {
            var columnConfig = new EntityColumn<EntityT>(columnName);
            Parent.Columns.Add(columnConfig);
            return columnConfig;
        }

        #endregion

        #region Appended
        public EntityColumnCollection<EntityT> AppendedColumns { get; private set; }
        public EntityColumn<EntityT> Append(string columnName)
        {
            EntityColumn<EntityT> columnConfig = new EntityColumn<EntityT>(columnName) { Parent = this.Parent };
            AppendedColumns.Add(columnConfig);
            return columnConfig;
        }

        void OnAppendedColumnsInserted(object sender, ItemEventArgs<IEnumerable<EntityColumn<EntityT>>> e)
        {
            var appendedColumns = e.Item;
            if (appendedColumns != null && appendedColumns.Any())
            { appendedColumns.ForEach(c => c.Parent = this.Parent); }
        }

        #endregion

        #region Import
        public DataGrid Import()
        {
            if (Parent == null)
            { throw new InvalidOperationException(); }
            return Parent.Import();
        }

        public CollectionBase<EntityT> ToCollection()
        {
            if (Parent == null)
            { throw new InvalidOperationException(); }
            return Parent.ToCollection();
        }

        #endregion

        #region Parent
        internal EntityFormatter<EntityT> Parent { get; set; }

        #endregion

        #region Serialization
        public override void FromElement(XElement element)
        {
            base.FromElement(element);

            // Appended Columns
            XNamespace xmlns = element.Name.Namespace;
            element.Elements(xmlns + "Append").ForEach(delegate(XElement columnNode)
            {
                EntityColumn<EntityT> column = AppendedColumns.New();
                column.FromElement(columnNode);
            });
        }

        #endregion
    }

}
