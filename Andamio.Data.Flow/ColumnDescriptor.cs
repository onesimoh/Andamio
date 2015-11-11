using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Xml.Linq;

using Andamio;
using Andamio.Collections;
using Andamio.Expressions;

namespace Andamio.Data
{
    #region Collection
    public class ColumnDescriptorCollection : CollectionBase<ColumnDescriptor>
    {
        #region Constructors
        public ColumnDescriptorCollection()
            : base()
        {
        }

        public ColumnDescriptorCollection(IEnumerable<ColumnDescriptor> columns) 
            : base(columns)        
        {
        }

        #endregion

        #region Indexer
        public ColumnDescriptor this[string columnName]
        {
            get
            {
                return this.SingleOrDefault(match => match.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            }
        }

        #endregion
    }

    #endregion


    [DebuggerDisplay("{ColumnName}")]
    public abstract class ColumnDescriptor
    {
        #region Constructors
        protected ColumnDescriptor()
        {
            ColumnSize = -1;
            ColumnPosition = -1;
            AllowNull = false;
            ColumnType = typeof(String);
            Strict = false;
        }

        protected ColumnDescriptor(string columnName) : this()
        {
            if (columnName.IsNullOrBlank()) throw new ArgumentNullException("columnName");
            ColumnName = columnName;
            BindingName = columnName;
        }

        #endregion

        #region Properties
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
        public int ColumnSize { get; set; }
        public int ColumnPosition { get; set; }
        public bool AllowNull { get; set; }        
        public bool Strict { get; set; }
        public object Default { get; set; }
        public string DisplayFormat { get; set; }
        public string ParseFormat { get; set; }
        public object CellValue { get; set; }
        public string ColumnMap { get; set; }

        #endregion

        #region Binding
        public string BindingName { get; protected set; }
        public System.Linq.Expressions.Expression BindingExpression { get; protected set; }

        private Type _bindingType;
        public Type BindingType
        {
            get { return _bindingType ?? ColumnType; }
            protected set { _bindingType = value; }
        }

        #endregion

        #region Serialization
        public virtual void FromElement(XElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            XNamespace xmlns = element.Name.Namespace;

            ColumnName = element.Attribute("name").Value;
            ColumnSize = element.Attribute("size").Optional<int>(-1);
            ColumnPosition = element.Attribute("position").Optional<int>(-1);
            AllowNull = element.Attribute("null").OptionalBool();
            Strict = element.Attribute("strict").OptionalBool();
            DisplayFormat = element.Attribute("display-format").Optional();
            ParseFormat = element.Attribute("parse-format").Optional();
            BindingName = element.Attribute("bind").Optional(ColumnName);
            ColumnMap = element.Attribute("column-map").Optional();

            string columnType = element.Attribute("type").Value;
            ColumnType = columnType.ToType();
            if (ColumnType == null)
            {
                throw new ColumnTypeException(String.Format("Invalid Column Type '{0}' for Column '{1}'.", columnType, ColumnName));
            }
            
            string bindingType = element.Attribute("bind-type").Optional(columnType);
            BindingType = bindingType.ToType();
            if (BindingType == null)
            {
                throw new ColumnTypeException(String.Format("Invalid Binding Type '{0}' for Column '{1}'.", bindingType, ColumnName));
            }

            string value = element.Attribute("value").Optional();
            if (value != null)
            {
                Andamio.Expressions.Expression expression;
                if (!Andamio.Expressions.DynamicExpression.IsReserved(value, out expression))
                {
                    object cellValue;
                    if (!value.TryParse(BindingType, out cellValue))
                    {
                        throw new ColumnTypeException(String.Format("Value '{0}' for Column '{1}' cannot be converted to '{2}'."
                            , value
                            , ColumnName
                            , BindingType.FriendlyName()));
                    }
                    CellValue = cellValue;                                        
                }
                else
                {
                    CellValue = expression.Value;
                }
            }
        }

        #endregion
    }
}
