using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Andamio;

namespace Andamio.Pdf
{
    public enum PdfUnitType
    {
        Unknown,
        Percentage,
        Absolute,
    }

    public static class PdfUnitTypeExtensions
    {
        public static bool IsUnknown(this PdfUnitType type)
        {
            return type == PdfUnitType.Unknown;
        }
        public static bool IsPercentage(this PdfUnitType type)
        {
            return type == PdfUnitType.Percentage;
        }
        public static bool IsAbsolute(this PdfUnitType type)
        {
            return type == PdfUnitType.Absolute;
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public struct PdfUnit
    {
        #region Constructors
        private PdfUnit(float value) : this()
        {
            this.Value = value;
            this.Type = PdfUnitType.Absolute;
        }

        private PdfUnit(float value, PdfUnitType type) : this()
        {
            this.Value = value;
            this.Type = type;
        }

        public static PdfUnit SetPercentageValue(float value)
        {
            return new PdfUnit(value) { Type = PdfUnitType.Percentage };            
        }

        public static PdfUnit SetAbsoluteValue(float value)
        {
            return new PdfUnit(value);
        }

        #endregion

        #region Blank
        public static readonly PdfUnit Blank = new PdfUnit(-1, PdfUnitType.Unknown);
        public bool IsBlank
        {
            get { return (Value == -1) && Type.IsUnknown(); }
        }

        #endregion

        #region Parse
        private static readonly Regex PercentageRegEx = new Regex(@"(?<unit>\d+(\.\d+)?)%", RegexOptions.Singleline);
        private static readonly Regex AbsoluteRegEx = new Regex(@"(?<unit>\d+(\.\d+)?)", RegexOptions.Singleline);

        public static PdfUnit Parse(string value)
        {
            if (value.IsNullOrBlank())
            {
                return PdfUnit.Blank; 
            }
            else if (PercentageRegEx.IsMatch(value))
            {
                Match unitRegExMatch = PercentageRegEx.Match(value);
                float unit = float.Parse(unitRegExMatch.Groups["unit"].Value);
                return new PdfUnit(unit, PdfUnitType.Percentage);
            }
            else if (AbsoluteRegEx.IsMatch(value))
            {
                Match unitRegExMatch = AbsoluteRegEx.Match(value);
                float unit = float.Parse(unitRegExMatch.Groups["unit"].Value);
                return new PdfUnit(unit);
            }
            else
            {
                throw new FormatException(String.Format("Specified Value '{0}' cannot be converted to a valid Unit.", value));
            }
        }

        #endregion

        #region Properties
        public float Value { get; private set; }
        public PdfUnitType Type { get; private set; }

        #endregion

        #region Operators
        public static bool operator ==(PdfUnit left, PdfUnit right)
        {
            return (left.Value == right.Value) && (left.Type == right.Type);
        }

        public static bool operator !=(PdfUnit left, PdfUnit right)
        {
            return (left.Value != right.Value) && (left.Type != right.Type);
        }

        public static implicit operator PdfUnit(float value)
        {
            return new PdfUnit(value);
        }

        public static implicit operator PdfUnit(string value)
        {
            if (value.IsNullOrBlank())
            { return PdfUnit.Blank; }
            return PdfUnit.Parse(value);
        }

        public override bool Equals(object obj)
        {
            if (obj is PdfUnit)
            { return this == (PdfUnit) obj; }
            return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        #endregion

        #region Format
        public override string ToString()
        {
            if (!IsBlank)
            {
                return (Type.IsPercentage()) ? String.Format("{0}%", Value) : Value.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion
    }
}
