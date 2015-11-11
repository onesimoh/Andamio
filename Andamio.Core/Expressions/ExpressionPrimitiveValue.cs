using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Expressions
{
    public class ExpressionPrimitiveValue<T> : Expression where T : struct
    {
        #region Constructors
        public ExpressionPrimitiveValue(string name, T value) : base(name, value)
        {
        }

        #endregion       

        #region Invoke
        public override object Invoke()
        {
            return Value;
        }

        #endregion
    }

    #region String
    public sealed class StringExpression : Expression
    {
        #region Constructors
        public StringExpression(string name, string value)
            : base(name, value)
        {
        }
        
        #endregion

        #region Format
        public override string Format(string format)
        {
            string formattedExpression = String.Format("{{0:{0}}}", format);
            return String.Format(formattedExpression, Value);
        }

        #endregion

        #region Invoke
        public override object Invoke()
        {
            return Value as String;
        }

        #endregion
    }

    #endregion

    #region DateTime
    public sealed class DateTimeExpression : ExpressionPrimitiveValue<DateTime>
    {
        public DateTimeExpression(string name, DateTime value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region UInt16
    public sealed class UInt16Expression : ExpressionPrimitiveValue<UInt16>
    {
        public UInt16Expression(string name, UInt16 value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region UInt32
    public sealed class UInt32Expression : ExpressionPrimitiveValue<UInt32>
    {
        public UInt32Expression(string name, UInt32 value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region UInt64
    public sealed class UInt64Expression : ExpressionPrimitiveValue<UInt64>
    {
        public UInt64Expression(string name, UInt64 value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Int16
    public sealed class Int16Expression : ExpressionPrimitiveValue<Int16>
    {
        public Int16Expression(string name, Int16 value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Int32
    public sealed class Int32Expression : ExpressionPrimitiveValue<Int32>
    {
        public Int32Expression(string name, Int32 value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Int64
    public sealed class Int64Expression : ExpressionPrimitiveValue<Int64>
    {
        public Int64Expression(string name, Int64 value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Decimal
    public sealed class DecimalExpression : ExpressionPrimitiveValue<Decimal>
    {
        public DecimalExpression(string name, Decimal value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Double
    public sealed class DoubleExpression : ExpressionPrimitiveValue<Double>
    {
        public DoubleExpression(string name, Double value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Single
    public sealed class SingleExpression : ExpressionPrimitiveValue<Single>
    {
        public SingleExpression(string name, Single value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Guid
    public sealed class GuidExpression : ExpressionPrimitiveValue<Guid>
    {
        public GuidExpression(string name, Guid value)
            : base(name, value)
        {
        }
    }

    #endregion
}
