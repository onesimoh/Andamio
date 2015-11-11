using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Andamio.Expressions
{
    public sealed class DynamicExpression
    {
        #region Constructors
        static DynamicExpression()
        {
            Reserved.Add("Today", () => DateTime.Today);
            Reserved.Add("Now", () => DateTime.Now);
            Reserved.Add("UtcNow", () => DateTime.UtcNow);
            Reserved.Add("Guid", () => Guid.NewGuid());
        }

        public DynamicExpression()
        {
            Values = new ExpressionsCollection(Reserved);
        }

        #endregion

        #region Values
        public static readonly ExpressionsCollection Reserved = new ExpressionsCollection();
        public ExpressionsCollection Values { get; private set; }

        #endregion

        #region Eval
        private static readonly Regex ExpressionRegex = new Regex(@"\{(?<var>[A-Za-z0-9_]+)(:(?<format>[A-Za-z/_-]+))*?\}");
        public static bool Match(string expression)
        {
            return ExpressionRegex.IsMatch(expression);
        }

        public static void AssertMatch(string expression)
        {
            if (!DynamicExpression.Match(expression))
            { throw new InvalidOperationException(String.Format("'{0}' Is Not a valid Expression.", expression)); }
        }

        public static bool IsReserved(string expression, out Expression reservedExpression)
        {
            reservedExpression = null;
            if (!Match(expression)) return false;            

            var matches = ExpressionRegex.Matches(expression);
            if (matches.Count == 1)
            {
                Match match = matches[0];
                string exprName = match.Groups["var"].Value;
                if (!Reserved.Contains(exprName))
                {
                    return false;
                }
                reservedExpression = Reserved[exprName];
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public string Eval(string expression)
        {
            string evaluatedExpression = expression;
            if (!expression.IsNullOrBlank() && DynamicExpression.Match(expression))
            {
                foreach (Match match in ExpressionRegex.Matches(expression))
                {
                    string name = match.Groups["var"].Value;
                    if (Values.Contains(name))
                    {
                        string originalExpr = match.ToString();
                        string format = match.Groups["format"].Value;

                        Expression expressionValue = Values[name];
                        string formattedExpr = expressionValue.Format(format);
                        evaluatedExpression = evaluatedExpression.Replace(originalExpr, formattedExpr);
                    }
                }
            }

            return evaluatedExpression;
        }

        #endregion
    }
}
