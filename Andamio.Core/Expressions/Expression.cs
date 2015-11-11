using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Andamio;
using Andamio.Collections;

namespace Andamio.Expressions
{
    [DebuggerDisplay("{Name}")]
    public abstract class Expression
    {
        #region Constructors
        private Expression()
        {
        }

        public Expression(string name, object value) : this()
        {
            if (name.IsNullOrBlank()) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");
            Name = name;
            Value = value;
        }

        #endregion

        #region Properties
        public string Name { get; private set; }
        public object Value { get; private set; }

        #endregion

        #region Format
        public virtual string Format(string format)
        {
            object value = Invoke();
            if (!format.IsNullOrBlank())
            {
                string formattedExpression = String.Format("{{0:{0}}}", format);
                return String.Format(formattedExpression, value);
            }
            else
            {
                return (value != null) ? value.ToString() : String.Empty;
            }
        }

        public virtual string Format()
        {
            object value = Invoke();
            return (value != null) ? value.ToString() : String.Empty;
        }

        #endregion

        #region Invoke
        public abstract object Invoke();
        
        #endregion
    }

    [DebuggerDisplay("Count={Count}")]
    public class ExpressionsCollection : CollectionBase<Expression>
    {
        #region Constructors
        public ExpressionsCollection() : base()
        {
        }

        public ExpressionsCollection(IEnumerable<Expression> values) : base(values)
        {
        }

        #endregion

        #region Indexers
        public Expression this[string name]
        {
            get
            {
                var expressions = this.Where(match => match.Name == name);
                if (!expressions.Any())
                { 
                    throw new KeyNotFoundException(String.Format("'{0}' is not a valid expression.", name)); 
                }
                if (expressions.Count() > 1)
                {
                    throw new KeyNotFoundException(String.Format("More than one expression was found for '{0}'.", name)); 
                }
                return expressions.First();
            }
        }

        #endregion

        #region Methods
        public bool Contains(string name)
        {
            return this.Any(match => match.Name == name);
        }

        #endregion

        #region DateTime
        public DateTimeExpression Add(string name, DateTime value)
        {
            var expressionValue = new DateTimeExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        public DateTimeFunctionalExpression Add(string name, Func<DateTime> value)
        {
            var expressionValue = new DateTimeFunctionalExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        #endregion

        #region String
        public StringExpression Add(string name, string value)
        {
            var expressionValue = new StringExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        public StringFunctionalExpression Add(string name, Func<string> value)
        {
            var expressionValue = new StringFunctionalExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        #endregion

        #region Integer
        public Int32Expression Add(string name, int value)
        {
            var expressionValue = new Int32Expression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        public Int32FunctionalExpression Add(string name, Func<int> value)
        {
            var expressionValue = new Int32FunctionalExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        #endregion

        #region Decimal
        public DecimalExpression Add(string name, decimal value)
        {
            var expressionValue = new DecimalExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        public DecimalFunctionalExpression Add(string name, Func<decimal> value)
        {
            var expressionValue = new DecimalFunctionalExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        #endregion

        #region Float
        public SingleExpression Add(string name, float value)
        {
            var expressionValue = new SingleExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        public SingleFunctionalExpression Add(string name, Func<float> value)
        {
            var expressionValue = new SingleFunctionalExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        #endregion

        #region Double
        public DoubleExpression Add(string name, double value)
        {
            var expressionValue = new DoubleExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        public DoubleFunctionalExpression Add(string name, Func<double> value)
        {
            var expressionValue = new DoubleFunctionalExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        #endregion

        #region Guid
        public GuidExpression Add(string name, Guid value)
        {
            var expressionValue = new GuidExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        public GuidFunctionalExpression Add(string name, Func<Guid> value)
        {
            var expressionValue = new GuidFunctionalExpression(name, value);
            Add(expressionValue);
            return expressionValue;
        }

        #endregion
    }
}
