using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Expressions
{
    public class ExpressionFunctionalValue<T> : Expression where T : struct
    {
        #region Constructors
        public ExpressionFunctionalValue(string name, Func<T> value) : base(name, value)
        {
        }

        #endregion

        #region Invoke
        public override object Invoke()
        {
            return ((Func<T>) Value).Invoke();
        }

        #endregion
    }

    #region String
    public sealed class StringFunctionalExpression : Expression
    {
        #region Constructors
        public StringFunctionalExpression(string name, Func<string> value)
            : base(name, value)
        {
        }

        #endregion

        #region Invoke
        public override object Invoke()
        {
            return ((Func<string>) Value).Invoke();
        }

        #endregion
    }

    #endregion

    #region DateTime
    public sealed class DateTimeFunctionalExpression : ExpressionFunctionalValue<DateTime>
    {
        public DateTimeFunctionalExpression(string name, Func<DateTime> value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region UInt16
    public sealed class UInt16Functional : ExpressionFunctionalValue<UInt16>
    {
        public UInt16Functional(string name, Func<UInt16> value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region UInt32
    public sealed class UInt32Functional : ExpressionFunctionalValue<UInt32>
    {
        public UInt32Functional(string name, Func<UInt32> value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region UInt64
    public sealed class UInt64Functional : ExpressionFunctionalValue<UInt64>
    {
        public UInt64Functional(string name, Func<UInt64> value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Int16
    public sealed class Int16Functiona : ExpressionFunctionalValue<Int16>
    {
        public Int16Functiona(string name, Func<Int16> value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Int32
    public sealed class Int32FunctionalExpression : ExpressionFunctionalValue<Int32>
    {
        public Int32FunctionalExpression(string name, Func<Int32> value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Int64
    public sealed class Int64Functional : ExpressionFunctionalValue<Int64>
    {
        public Int64Functional(string name, Func<Int64> value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Decimal
    public sealed class DecimalFunctionalExpression : ExpressionFunctionalValue<Decimal>
    {
        public DecimalFunctionalExpression(string name, Func<Decimal> value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Double
    public sealed class DoubleFunctionalExpression : ExpressionFunctionalValue<Double>
    {
        public DoubleFunctionalExpression(string name, Func<Double> value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Single
    public sealed class SingleFunctionalExpression : ExpressionFunctionalValue<Single>
    {
        public SingleFunctionalExpression(string name, Func<Single> value)
            : base(name, value)
        {
        }
    }

    #endregion

    #region Guid
    public sealed class GuidFunctionalExpression : ExpressionFunctionalValue<Guid>
    {
        public GuidFunctionalExpression(string name, Func<Guid> value)
            : base(name, value)
        {
        }
    }

    #endregion
}
