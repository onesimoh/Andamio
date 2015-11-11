using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio
{
    /// <summary>
    /// Represents an instance of an OLE Automation Date Time.
    /// </summary>
    public struct OleDateTime
    {
        #region Constructors
        public OleDateTime(double d)
        {
            Value = DateTime.FromOADate(d);
        }

        #endregion

        #region Operators
        /// <summary>
        /// Subtracts a specified date and time from another specified date and time and returns a time interval.
        /// </summary>
        /// <param name="d1">The date and time value to subtract from (the minuend).</param>
        /// <param name="d2">The date and time value to subtract (the subtrahend).</param>
        /// <returns>The time interval between d1 and d2; that is, d1 minus d2.</returns>
        public static TimeSpan operator -(OleDateTime d1, OleDateTime d2)
        {            
            return  (d1.Value - d2.Value);
        }

        /// <summary>
        /// Adds a specified time interval to a specified date and time, yielding a new date and time.
        /// </summary>
        /// <param name="d1">The date and time value to add.</param>
        /// <param name="d2">The date and time value to add.</param>
        /// <returns>An object that is the sum of the values of d and t.</returns>
        public static OleDateTime operator +(OleDateTime d1, TimeSpan t)
        {
            return new OleDateTime(((DateTime) (d1.Value + t)).ToOADate());
        }

        /// <summary>
        /// Determines whether two specified instances are not equal.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if d1 and d2 do not represent the same date and time; otherwise, false.</returns>
        public static bool operator !=(OleDateTime d1, OleDateTime d2)
        {
            return d1.Value != d2.Value;
        }

        /// <summary>
        /// Determines whether one specified OleDateTime is less than another specified OleDateTime.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if t1 is less than t2; otherwise, false.</returns>
        public static bool operator <(OleDateTime d1, OleDateTime d2)
        {
            return d1.Value < d2.Value;
        }

        /// <summary>
        /// Determines whether one specified OleDateTime is less than or equal to another specified OleDateTime.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if t1 is less than or equal to t2; otherwise, false.</returns>
        public static bool operator <=(OleDateTime d1, OleDateTime d2)        
        {
            return d1.Value <= d2.Value;
        }                 

        /// <summary>
        /// Determines whether two specified instances of System.DateTime are equal.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if d1 and d2 represent the same date and time; otherwise, false.</returns>
        public static bool operator ==(OleDateTime d1, OleDateTime d2)
        {
            return d1.Value == d2.Value;
        }
        
        /// <summary>
        /// Determines whether one specified System.DateTime is greater than another
        /// specified System.DateTime.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if t1 is greater than t2; otherwise, false.</returns>
        public static bool operator >(OleDateTime d1, OleDateTime d2)
        {
            return d1.Value > d2.Value;
        }   
        
        /// <summary>
        /// Determines whether one specified System.DateTime is greater than or equal
        /// to another specified System.DateTime.
        /// </summary>
        /// <param name="d1">The first object to compare.</param>
        /// <param name="d2">The second object to compare.</param>
        /// <returns>true if t1 is greater than or equal to t2; otherwise, false.</returns>
        public static bool operator >=(OleDateTime d1, OleDateTime d2)
        {
            return d1.Value >= d2.Value;
        }

        #endregion

        #region Conversion
        public static explicit operator OleDateTime(DateTime dt)
        {
            return new OleDateTime(dt.ToOADate());
        }

        #endregion

        #region Properties
        /// <summary>
        /// DateTime representation of the OleDateTime instance.
        /// </summary>
        public readonly DateTime Value;

        /// <summary>
        /// Gets the date component of this instance.
        /// </summary>
        public DateTime Date { get { return Value.Date; } }
        
        /// <summary>
        /// Gets the day of the month represented by this instance.
        /// </summary>
        public int Day { get { return Value.Day; } }
        
        /// <summary>
        /// Gets the day of the week represented by this instance.
        /// </summary>
        public DayOfWeek DayOfWeek { get { return Value.DayOfWeek; } }

        /// <summary>
        /// Gets the day of the year represented by this instance.
        /// </summary>
        public int DayOfYear { get { return Value.DayOfYear; } }
        
        /// <summary>
        /// Gets the hour component of the date represented by this instance.
        /// </summary>
        public int Hour { get { return Value.Hour; } }
                
        /// <summary>
        /// Gets the milliseconds component of the date represented by this instance.
        /// </summary>
        public int Millisecond { get { return Value.Millisecond; } }
        
        /// <summary>
        /// Gets the minute component of the date represented by this instance.
        /// </summary>
        public int Minute { get { return Value.Minute; } }

        /// <summary>
        /// Gets the month component of the date represented by this instance.
        /// </summary>
        public int Month { get { return Value.Month; } }
        
        /// <summary>
        /// Gets the seconds component of the date represented by this instance.
        /// </summary>
        public int Second { get { return Value.Second; } }

        /// <summary>
        /// Gets the number of ticks that represent the date and time of this instance.
        /// </summary>
        public long Ticks { get { return Value.Ticks; } }
        
        /// <summary>
        /// Gets the time of day for this instance.
        /// </summary>
        public TimeSpan TimeOfDay { get { return Value.TimeOfDay; } }

        /// <summary>
        /// Gets a System.DateTime object that is set to the current date and time on this computer, expressed as the local time.
        /// </summary>
        public static OleDateTime Now { get { return (OleDateTime) DateTime.Now; } }

        /// <summary>
        /// Gets the current date.
        /// </summary>
        public static OleDateTime Today { get { return (OleDateTime)DateTime.Today; } }

        /// <summary>
        /// Gets a System.DateTime object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        public static OleDateTime UtcNow { get { return (OleDateTime)DateTime.UtcNow; } }

        /// <summary>
        /// Gets the year component of the date represented by this instance.
        /// </summary>
        public int Year { get { return Value.Year; } }

        #endregion

        #region Methods
        /// <summary>
        /// Returns a new System.DateTime that adds the value of the specified System.TimeSpan to the value of this instance.
        /// </summary>
        /// <param name="value">A positive or negative time interval.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the time interval represented by value.</returns>
        public OleDateTime Add(TimeSpan value)
        {
            return (OleDateTime) Value.Add(value);
        }

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of days to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional days. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of days represented by value.</returns>
        public OleDateTime AddDays(double value)
        {
            return (OleDateTime) Value.AddDays(value);
        }

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of hours to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional hours. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of hours represented by value.</returns>
        public OleDateTime AddHours(double value)
        {
            return (OleDateTime) Value.AddHours(value);
        }

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of milliseconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional milliseconds. The value parameter can be negative or positive. Note that this value is rounded to the nearest integer.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of milliseconds represented by value.</returns>
        public OleDateTime AddMilliseconds(double value)
        {
            return (OleDateTime) Value.AddMilliseconds(value);
        }

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of minutes to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional minutes. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of minutes represented by value.</returns>
        public OleDateTime AddMinutes(double value)
        {
            return (OleDateTime) Value.AddMinutes(value);
        }

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of months to the value of this instance.
        /// </summary>
        /// <param name="months">A number of months. The months parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and months.</returns>
        public OleDateTime AddMonths(int months)
        {
            return (OleDateTime) Value.AddMonths(months);
        }

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of seconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional seconds. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of seconds represented by value.</returns>
        public OleDateTime AddSeconds(double value)
        {
            return (OleDateTime) Value.AddSeconds(value);
        }

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of ticks to the value of this instance.
        /// </summary>
        /// <param name="value">A number of 100-nanosecond ticks. The value parameter can be positive or negative.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the time represented by value.</returns>
        public OleDateTime AddTicks(long value)
        {
            return (OleDateTime) Value.AddTicks(value);
        }

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of years to the value of this instance.
        /// </summary>
        /// <param name="value">A number of years. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of years represented by value.</returns>
        public OleDateTime AddYears(int value)
        {
            return (OleDateTime) Value.AddYears(value);
        }

        /// <summary>
        /// Subtracts the specified date and time from this instance.
        /// </summary>
        /// <param name="dt">The date and time value to subtract.</param>
        /// <returns>A time interval that is equal to the date and time represented by this instance minus the date and time represented by value.</returns>
        public TimeSpan Subtract(OleDateTime dt)
        {
            return Value.Subtract(dt.Value);
        }

        /// <summary>
        /// Subtracts the specified duration from this instance.
        /// </summary>
        /// <param name="value">The time interval to subtract.</param>
        /// <returns>An object that is equal to the date and time represented by this instance minus the time interval represented by value.</returns>
        public OleDateTime Subtract(TimeSpan value)
        {
            return (OleDateTime) Value.Subtract(value);
        }

        #endregion

        #region Compare
        //
        // Summary:
        //     Compares two instances of System.DateTime and returns an integer that indicates
        //     whether the first instance is earlier than, the same as, or later than the
        //     second instance.
        //
        // Parameters:
        //   t1:
        //     The first object to compare.
        //
        //   t2:
        //     The second object to compare.
        //
        // Returns:
        //     A signed number indicating the relative values of t1 and t2.Value Type Condition
        //     Less than zero t1 is earlier than t2. Zero t1 is the same as t2. Greater
        //     than zero t1 is later than t2.
        public static int Compare(OleDateTime t1, OleDateTime t2)
        {
            return DateTime.Compare(t1.Value, t2.Value);
        }

        //
        // Summary:
        //     Compares the value of this instance to a specified System.DateTime value
        //     and returns an integer that indicates whether this instance is earlier than,
        //     the same as, or later than the specified System.DateTime value.
        //
        // Parameters:
        //   value:
        //     The object to compare to the current instance.
        //
        // Returns:
        //     A signed number indicating the relative values of this instance and the value
        //     parameter.Value Description Less than zero This instance is earlier than
        //     value. Zero This instance is the same as value. Greater than zero This instance
        //     is later than value.
        public int CompareTo(OleDateTime d)
        {
            return Value.CompareTo(d.Value);
        }

        /// <summary>
        /// Compares the value of this instance to a specified object.
        /// </summary>
        /// <param name="value">A boxed object to compare, or null.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance.
        /// </returns>
        public int CompareTo(object value)
        {
            return Value.CompareTo(value);
        }

        //
        // Summary:
        //     Returns a value indicating whether the value of this instance is equal to
        //     the value of the specified System.DateTime instance.
        //
        // Parameters:
        //   value:
        //     The object to compare to this instance.
        //
        // Returns:
        //     true if the value parameter equals the value of this instance; otherwise,
        //     false.
        public bool Equals(OleDateTime d)
        {
            return Value.Equals(d.Value);
        }

        //
        // Summary:
        //     Returns a value indicating whether this instance is equal to a specified
        //     object.
        //
        // Parameters:
        //   value:
        //     The object to compare to this instance.
        //
        // Returns:
        //     true if value is an instance of System.DateTime and equals the value of this
        //     instance; otherwise, false.
        public override bool Equals(object value)
        {
            return Value.Equals(value);
        }

        //
        // Summary:
        //     Returns a value indicating whether two System.DateTime instances have the
        //     same date and time value.
        //
        // Parameters:
        //   t1:
        //     The first object to compare.
        //
        //   t2:
        //     The second object to compare.
        //
        // Returns:
        //     true if the two values are equal; otherwise, false.
        public static bool Equals(OleDateTime t1, OleDateTime t2)
        {
            return DateTime.Equals(t1.Value, t2.Value);
        }


        //
        // Summary:
        //     Returns the hash code for this instance.
        //
        // Returns:
        //     A 32-bit signed integer hash code.
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        //
        // Summary:
        //     Returns the System.TypeCode for value type System.DateTime.
        //
        // Returns:
        //     The enumerated constant, System.TypeCode.DateTime.
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        #endregion

        #region Format
        //
        // Summary:
        //     Converts the value of the current System.DateTime object to local time.
        //
        // Returns:
        //     An object whose System.DateTime.Kind property is System.DateTimeKind.Local,
        //     and whose value is the local time equivalent to the value of the current
        //     System.DateTime object, or System.DateTime.MaxValue if the converted value
        //     is too large to be represented by a System.DateTime object, or System.DateTime.MinValue
        //     if the converted value is too small to be represented as a System.DateTime
        //     object.
        public OleDateTime ToLocalTime()
        {
            return (OleDateTime) Value.ToLocalTime();
        }

        //
        // Summary:
        //     Converts the value of the current System.DateTime object to its equivalent
        //     long date string representation.
        //
        // Returns:
        //     A string that contains the long date string representation of the current
        //     System.DateTime object.
        public string ToLongDateString()
        {
            return Value.ToLongDateString();
        }

        //
        // Summary:
        //     Converts the value of the current System.DateTime object to its equivalent
        //     long time string representation.
        //
        // Returns:
        //     A string that contains the long time string representation of the current
        //     System.DateTime object.
        public string ToLongTimeString()
        {
            return Value.ToLongTimeString();
        }

        //
        // Summary:
        //     Converts the value of the current System.DateTime object to its equivalent
        //     short date string representation.
        //
        // Returns:
        //     A string that contains the short date string representation of the current
        //     System.DateTime object.
        public string ToShortDateString()
        {
            return Value.ToShortDateString();
        }

        //
        // Summary:
        //     Converts the value of the current System.DateTime object to its equivalent
        //     short time string representation.
        //
        // Returns:
        //     A string that contains the short time string representation of the current
        //     System.DateTime object.
        public string ToShortTimeString()
        {
            return Value.ToShortTimeString();
        }

        //
        // Summary:
        //     Converts the value of the current System.DateTime object to its equivalent
        //     string representation.
        //
        // Returns:
        //     A string representation of the value of the current System.DateTime object.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The date and time is outside the range of dates supported by the calendar
        //     used by the current culture.
        public override string ToString()
        {
            return Value.ToString();
        }

        //
        // Summary:
        //     Converts the value of the current System.DateTime object to its equivalent
        //     string representation using the specified culture-specific format information.
        //
        // Parameters:
        //   provider:
        //     An object that supplies culture-specific formatting information.
        //
        // Returns:
        //     A string representation of value of the current System.DateTime object as
        //     specified by provider.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The date and time is outside the range of dates supported by the calendar
        //     used by provider.
        public string ToString(IFormatProvider provider)
        {
            return Value.ToString(provider);
        }

        //
        // Summary:
        //     Converts the value of the current System.DateTime object to its equivalent
        //     string representation using the specified format.
        //
        // Parameters:
        //   format:
        //     A standard or custom date and time format string (see Remarks).
        //
        // Returns:
        //     A string representation of value of the current System.DateTime object as
        //     specified by format.
        //
        // Exceptions:
        //   System.FormatException:
        //     The length of format is 1, and it is not one of the format specifier characters
        //     defined for System.Globalization.DateTimeFormatInfo.-or- format does not
        //     contain a valid custom format pattern.
        //
        //   System.ArgumentOutOfRangeException:
        //     The date and time is outside the range of dates supported by the calendar
        //     used by the current culture.
        public string ToString(string format)
        {
            return Value.ToString(format);
        }

        //
        // Summary:
        //     Converts the value of the current System.DateTime object to its equivalent
        //     string representation using the specified format and culture-specific format
        //     information.
        //
        // Parameters:
        //   format:
        //     A standard or custom date and time format string.
        //
        //   provider:
        //     An object that supplies culture-specific formatting information.
        //
        // Returns:
        //     A string representation of value of the current System.DateTime object as
        //     specified by format and provider.
        //
        // Exceptions:
        //   System.FormatException:
        //     The length of format is 1, and it is not one of the format specifier characters
        //     defined for System.Globalization.DateTimeFormatInfo.-or- format does not
        //     contain a valid custom format pattern.
        //
        //   System.ArgumentOutOfRangeException:
        //     The date and time is outside the range of dates supported by the calendar
        //     used by provider.
        public string ToString(string format, IFormatProvider provider)
        {
            return Value.ToString(format, provider);
        }

        #endregion
    }
}
