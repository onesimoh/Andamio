using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Andamio
{
    /// <summary>
    /// Extension methods for System.DateTime
    /// </summary>
    public static class DateTimeExtensions
    {
        #region Constants
        public static readonly string EmptyText = "(none)";
        public static readonly string MilitaryTimeFormat = "HH:mm:ss";
        public static readonly string TimeFormat = "hh:mm:ss tt";
        public static readonly string DateFormat = "MM/dd/yyyy";
        public static readonly string DateTimeFormat = "MM/dd/yyyy hh:mm tt";
        public static readonly string DateTimeSecondsFormat = "MM/dd/yyyy hh:mm:ss tt";
        #endregion

        #region Age
        /// <summary>
        /// Calculates an age relative to the current date/time.
        /// </summary>
        /// <param name="dateTime">The value to calculate an age for.</param>
        /// <returns>The age of birthdate relative to now.</returns>
        public static TimeSpan Age(this DateTime datetime, bool calendarDays = true)
        {
            TimeSpan span = (calendarDays) ? datetime - DateTime.Today : DateRange.BusinessDays(datetime, DateTime.Today);
            return span;
        }

        /// <summary>
        /// Calculates an age relative to the current date/time.
        /// </summary>
        /// <param name="dateTime">The value to calculate an age for.</param>
        /// <returns>The age of birthdate relative to now.</returns>
        public static TimeSpan Age(this DateTime? datetime, bool calendarDays = true)
        {
            return (datetime.HasValue) ? datetime.Value.Age(calendarDays) : TimeSpan.Zero;
        }

        #endregion

        #region Formatting
        public static string FormatDate(this DateTime? datetime)
        {
            return FormatDateTimeText(datetime, DateFormat);
        }

        public static string FormatDate(this DateTime datetime)
        {
            return FormatDateTimeText(datetime, DateFormat);
        }

        public static string FormatTime(this DateTime? datetime)
        {
            return FormatDateTimeText(datetime, TimeFormat);
        }

        public static string FormatTime(this DateTime datetime)
        {
            return FormatDateTimeText(datetime, TimeFormat);
        }

        public static string FormatDateTime(this DateTime? datetime)
        {
            return FormatDateTimeText(datetime, DateTimeFormat);
        }

        public static string FormatDateTime(this DateTime datetime)
        {
            return FormatDateTimeText(datetime, DateTimeFormat);
        }

        public static string FormatDateTimeSeconds(this DateTime? datetime)
        {
            return FormatDateTimeText(datetime, DateTimeSecondsFormat);
        }

        public static string FormatDateTimeSeconds(this DateTime datetime)
        {
            return FormatDateTimeText(datetime, DateTimeSecondsFormat);
        }

        public static string FormatMilitaryTime(this DateTime? datetime)
        {
            return FormatDateTimeText(datetime, MilitaryTimeFormat);
        }

        public static string FormatMilitaryTime(this DateTime datetime)
        {
            return FormatDateTimeText(datetime, MilitaryTimeFormat);
        }

        public static string FormatDateTimeText(this DateTime? datetime, string format)
        {
            if (datetime == null)
            { return EmptyText; }
            else
            { return FormatDateTimeText(datetime.Value, format); }
        }

        public static string FormatShortDateString(this DateTime? datetime)
        {
            if (datetime == null)
            { return null; }
            else
            { return datetime.Value.ToShortDateString(); }
        }

        public static string FormatDateTimeText(this DateTime datetime, string format)
        {
            return datetime.ToString(format);
        }

        public static string FormatElapsedTime(this DateTime datetime, bool calendarDays = true)
        {            
            if (datetime.IsToday())
            {
                return "Today";
            }
            else if (datetime.IsYesterday())
            {
                return "Yesterday";
            }
            else if (datetime.IsTomorrow())
            {
                return "Tomorrow";
            }            
            else
            {
                TimeSpan span = datetime.Age(calendarDays);
                return (span.Days < 0) ? String.Format("{0} ago", span.ToScaledString()) : span.ToScaledString();
            }
        }

        public static string FormatElapsedTime(this Nullable<DateTime> datetime, bool calendarDays = true)
        {
            return datetime.HasValue ? datetime.Value.FormatElapsedTime() : String.Empty;
        }

        #endregion

        #region Day (Custom Grouping)
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.UtcNow.Date;
        }

        public static bool IsYesterday(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1) == DateTime.UtcNow.Date;
        }

        public static bool IsTomorrow(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(-1) == DateTime.UtcNow.Date;
        }

        public static bool IsWeekend(this DateTime dateTime)
        {
            return (dateTime.DayOfWeek == DayOfWeek.Saturday) || (dateTime.DayOfWeek == DayOfWeek.Sunday);
        }

        public static DateTime GetBusinessDate(this DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Saturday)
            {
                return dateTime.AddDays(-1);
            }
            else if (dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return dateTime.AddDays(-2);
            }
            else
            {
                return dateTime;
            }
        }

        public static DateTime PreviousBusinessDate(this DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Saturday)
            {
                return dateTime.AddDays(-1);
            }
            else if (dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return dateTime.AddDays(-2);
            }
            else if (dateTime.DayOfWeek == DayOfWeek.Monday)
            {
                return dateTime.AddDays(-3);
            }
            else
            {
                return dateTime.AddDays(-1);
            }
        }

        #endregion

        #region Weekly (Custom Grouping)
        public static int GetWeekOfYear(this DateTime dateTime)
        {
            return GetWeekOfYear(dateTime, new USCulture());
        }

        public static int GetWeekOfYear(this DateTime dateTime, CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            { throw new ArgumentNullException("cultureInfo"); }

            DateTimeFormatInfo dateTimeFormatInfo = cultureInfo.DateTimeFormat;
            Calendar calendar = cultureInfo.Calendar;

            return calendar.GetWeekOfYear(dateTime, dateTimeFormatInfo.CalendarWeekRule, dateTimeFormatInfo.FirstDayOfWeek);
        }

        public static bool IsCurrentWeek(this DateTime dateTime)
        {
            return (dateTime.GetWeekOfYear() == DateTime.Today.GetWeekOfYear());
        }

        public static bool IsLastWeek(this DateTime dateTime)
        {
            return (dateTime.GetWeekOfYear() == DateTime.Today.GetWeekOfYear() - 1);
        }

        public static bool IsLastTwoWeeks(this DateTime dateTime)
        {
            return (dateTime.GetWeekOfYear() == DateTime.Today.GetWeekOfYear() - 2);
        }

        public static bool IsLastThreeWeeks(this DateTime dateTime)
        {
            return (dateTime.GetWeekOfYear() == DateTime.Today.GetWeekOfYear() - 3);
        }

        public static bool IsNextWeek(this DateTime dateTime)
        {
            return (dateTime.GetWeekOfYear() == DateTime.Today.GetWeekOfYear() + 1);
        }

        public static bool IsNextTwoWeeks(this DateTime dateTime)
        {
            return (dateTime.GetWeekOfYear() == DateTime.Today.GetWeekOfYear() + 2);
        }

        public static bool IsNextThreeWeeks(this DateTime dateTime)
        {
            return (dateTime.GetWeekOfYear() == DateTime.Today.GetWeekOfYear() + 3);
        }

        #endregion

        #region Monthy (Custom Grouping)
        public static bool IsThisMonth(this DateTime dateTime)
        {
            return (dateTime.Month == DateTime.Today.Month)
                && (dateTime.Year == DateTime.Today.Year);
        }
        public static bool IsNextMonth(this DateTime dateTime)
        {
            return (dateTime.Month == DateTime.Today.Month + 1)
                && (dateTime.Year == DateTime.Today.Year);
        }
        public static bool IsAfterNextMonth(this DateTime dateTime)
        {
            return (dateTime.Month == DateTime.Today.Month + 2)
                && (dateTime.Year == DateTime.Today.Year);
        }

        public static bool IsLastMonth(this DateTime dateTime)
        {
            return (dateTime.Month == DateTime.Today.Month - 1)
                && (dateTime.Year == DateTime.Today.Year);
        }
        public static bool IsBeforeLastMonth(this DateTime dateTime)
        {
            return (dateTime.Month <= DateTime.Today.Month - 2)
                && (dateTime.Year == DateTime.Today.Year);
        }

        public static int DaysInMonth(this DateTime dateTime)
        {
            return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
        }

        #endregion

        #region Quaterly (Custom Grouping)
        public static string GetQuarter(this DateTime dateTime)
        {
            int quarterMonth = (dateTime.Month % 3) == 0 ? (dateTime.Month / 3) : (dateTime.Month / 3) + 1;
            return String.Format("Q{0} {1}", quarterMonth, dateTime.Year);
        }
        
        public static string GetQuarter(this DateTime? dateTime)
        {
            return (dateTime.HasValue) ? GetQuarter(dateTime.Value) : String.Empty;
        }



        #endregion

        #region Parsing
        public static readonly Regex GlobusDateRegex = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Singleline);
        public static DateTime ParseGlobusDate(this string date)
        {
            DateTime dateTime;
            if (!date.TryParseGlobusDate(out dateTime))
            { throw new FormatException(String.Format("Invalid Globus Date Format: '{0}'", date)); }

            return dateTime;
        }

        public static bool TryParseGlobusDate(this string date, out DateTime dateTime)
        {
            dateTime = DateTime.MinValue;            
            if (date.IsNullOrBlank())
            { return false; }

            Match regExMatch = GlobusDateRegex.Match(date);
            if (!regExMatch.Success)
            { return false;  }

            int year = Int32.Parse(regExMatch.Groups["year"].Value);
            int month = Int32.Parse(regExMatch.Groups["month"].Value);
            int day = Int32.Parse(regExMatch.Groups["day"].Value);

            dateTime = new DateTime(year, month, day);

            return true;
        }

        public static DateTime? ParseEURDate(this string date)
        {
            if (date.IsNullOrBlank())
            { return null; }

            try
            {
                string[] formats = { "dd/MM/yy", "dd/MM/yyyy", "MM/dd/yyyy", "MM/dd/yy" };
                return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }


    #region US Culture
    public sealed class USCulture : CultureInfo
    {
        public USCulture() : base("en-US")
        {
        }
    }

    #endregion
}
