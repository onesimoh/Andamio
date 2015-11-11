using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Andamio
{
    /// <summary>
    /// Represents a Date Range.
    /// </summary>
    [Serializable]
    public class DateRange
    {
        #region Constructors
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public DateRange()
        {
            Kind = DateRangeKind.None;
        }

        /// <summary>
        /// Instantiates the class object and specified a date range.
        /// </summary>
        /// <param name="start">Starting date.</param>
        /// <param name="end">Ending Date.</param>
        public DateRange(DateTime? start, DateTime? end) : this()
        {
            Start = start;
            End = end;
            Kind = DateRangeKind.Custom;

            if (!ValidateRange())
            { throw new ArgumentException("Invalid Date Range."); }
        }

        #endregion

        #region Create (Static Methods)
        public static readonly DateRange Empty = new DateRange();
        public static DateRange Create(DateRangePrecision precision, int span, DateTime? dateTime = null)
        {
            DateTime dt = dateTime ?? DateTime.Today;

            DateRange dateRange;
            switch (precision)
            {
                case DateRangePrecision.Seconds:
                    dateRange = new DateRange(dt.Add(new TimeSpan(0, 0, span)), dt);
                    break;
                case DateRangePrecision.Minutes:
                    dateRange = new DateRange(dt.Add(new TimeSpan(0, span, 0)), dt);
                    break;
                case DateRangePrecision.Hours:
                    dateRange = new DateRange(dt.Add(new TimeSpan(span, 0, 0)), dt);
                    break;
                case DateRangePrecision.Days:
                    dateRange = new DateRange(dt.Add(new TimeSpan(span, 0, 0, 0, 0)), dt);
                    break;
                case DateRangePrecision.Months:
                    dateRange = new DateRange(dt.AddMonths(span), dt.AddDays(-1));
                    break;
                case DateRangePrecision.Year:
                    dateRange = new DateRange(dt.AddYears(span), dt);
                    break;
                default:
                    dateRange = DateRange.Empty;
                    break;
            }

            dateRange.Kind = DateRangeKind.Custom;

            return dateRange;
        }

        public static DateRange Create(DateRangeKind dateRangeKind)
        {
            DateTime now = DateTime.UtcNow;

            DateRange dateRange;
            switch (dateRangeKind)
            {
                case DateRangeKind.Today:                    
                    dateRange = new DateRange(now, now.Date.AddDays(1));
                    break;
                case DateRangeKind.Last7Days:
                    dateRange = Create(DateRangePrecision.Days, -7);
                    break;
                case DateRangeKind.Last30Days:
                    dateRange = Create(DateRangePrecision.Days, -30);
                    break;
                case DateRangeKind.Last90Days:
                    dateRange = Create(DateRangePrecision.Days, -90);
                    break;
                case DateRangeKind.Last6Months:
                    dateRange = Create(DateRangePrecision.Months, -6);
                    break;
                case DateRangeKind.Last12Months:
                    dateRange = Create(DateRangePrecision.Months, -12);
                    break;

                case DateRangeKind.MonthToDate:
                    dateRange = new DateRange(new DateTime(now.Year, now.Month, 1), now.Date.AddDays(1));
                    break;
                case DateRangeKind.QuarterToDate:
                    int quarterMonth = (now.Month % 3) == 0 ? now.Month : 3 * (now.Month / 3) + 1;
                    dateRange = new DateRange(new DateTime(now.Year, quarterMonth, 1), now.Date.AddDays(1));
                    break;
                case DateRangeKind.YearToDate:
                    dateRange = new DateRange(new DateTime(now.Year, 1, 1), now.Date.AddDays(1));
                    break;

                case DateRangeKind.PreviousMonth:
                    DateTime lastMonth = now.AddMonths(-1);
                    dateRange = new DateRange(new DateTime(lastMonth.Year, lastMonth.Month, 1), new DateTime(now.Year, now.Month, 1));
                    break;
                case DateRangeKind.PreviousQuarter:
                    DateTime lastQuarter = now.AddMonths(-3);
                    int lastQuarterMonth = (lastQuarter.Month % 3) == 0 ? lastQuarter.Month : 3 * (lastQuarter.Month / 3) + 1;
                    lastQuarter = new DateTime(lastQuarter.Year, lastQuarterMonth, 1);
                    dateRange = new DateRange(lastQuarter, lastQuarter.AddMonths(3));                    
                    break;
                case DateRangeKind.PreviousYear:
                    DateTime lastYear = now.AddYears(-1);
                    dateRange = new DateRange(new DateTime(lastYear.Year, lastYear.Month, 1), new DateTime(now.Year, 1, 1));
                    break;
                default:
                    dateRange = DateRange.Empty;
                    break;
            }

            dateRange.Kind = dateRangeKind;

            return dateRange;
        }

        public static DateRange[] CreateDateRangesForRecentQuarters()
        {
            return CreateDateRangesForRecentQuarters(4);
        }

        public static DateRange[] CreateDateRangesForRecentQuarters(int range)
        {
            DateTime now = DateTime.UtcNow.Date;
            DateRange[] dateRanges = new DateRange[range];
            for (int i = 0; i < range; i++)
            {
                dateRanges[i] = DateRange.CreateQuarterDateRange(now);
                now = now.AddMonths(-3);
            }

            return dateRanges;
        }

        public static DateRange CreateQuarterDateRange(DateTime dateTime)
        {
            int quarterMonth = (dateTime.Month % 3) == 0 ? dateTime.Month : 3 * (dateTime.Month / 3) + 1;

            DateTime quarterDate = new DateTime(dateTime.Year, quarterMonth, 1);
            DateRange dateRange = new DateRange(quarterDate, quarterDate.AddMonths(3)) { Kind = DateRangeKind.Quarter };

            return dateRange;
        }

        public static DateRange[] CreateForMonthsInRecentQuarters(int range)
        {            
            DateTime today = DateTime.UtcNow.Date;
            int quarterMonth = ((today.Month % 3) == 0 ? today.Month : 3 * ((today.Month / 3) + 1));

            DateTime dateTime = new DateTime(DateTime.Today.Year, quarterMonth, 1);

            List<DateRange> dateRanges = new List<DateRange>();
            for (int i = 0; i < range * 3; i++)
            {
                DateRange dateRange = new DateRange();
                dateRange.Parse(dateTime.ToString("MMMyyyy"));
                dateRanges.Add(dateRange);

                dateTime = dateTime.AddMonths(-1);
            }

            //DateTime dateTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            
            //for (int i = 0; i < months; i++)
            //{
            //    DateRange dateRange = new DateRange();
            //    dateRange.Parse(dateTime.AddMonths(-1).ToString("MMMyyyy"));
            //    dateRanges[i] = dateRange;

            //    dateTime = dateTime.AddMonths(-1);
            //}

            return dateRanges.ToArray();
        }

        public static DateRange[] ForLast12Months()
        {
            return ForLastNMonths(12);
        }

        public static DateRange[] ForLastNMonths(int months)
        {
            int numberOfDays = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            DateTime firstDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime lastDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, numberOfDays);
            List<DateRange> dateRanges = new List<DateRange>();

            dateRanges.Add(new DateRange(firstDay, DateTime.Today));

            for (int i = 0; i < months - 1; ++i)
            {
                DateRange dateRange = DateRange.Create(DateRangePrecision.Months, -1, dateRanges[i].Start);
                dateRange.Kind = DateRangeKind.Month;
                dateRanges.Add(dateRange);
            }

            return dateRanges.ToArray();
        }

        #endregion

        #region Range
        /// <summary>
        /// Gets starting date for the range.
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// Gets ending date for the range.
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Indicates whether date range is valid.
        /// </summary>
        /// <remarks>
        /// Returns true if ending date is greater than or equal than starting date; false otherwise.
        /// </remarks>
        public bool ValidateRange()
        {
            if ((Start.HasValue && End.HasValue) && (End.Value >= Start.Value))
            {
                return true;
            }
            else if (Start.HasValue && !End.HasValue)
            {
                return true;
            }
            else if (!Start.HasValue && End.HasValue)
            {
                return true;
            }
            else if (!Start.HasValue && !End.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DateRangeKind Kind { get; private set; }

        #endregion

        #region Duration
        private static readonly TimeSpan OneDay = TimeSpan.FromDays(1);
        public bool IsAllDay
        {
            get
            {
                return ((Duration == OneDay) && (Start.Value.Date.TimeOfDay == TimeSpan.Zero));
            }
        }

        public bool ExpandsMultipleDays
        {
            get
            {
                return ((Start.Value.Date != End.Value.Date) && (!IsAllDay));
            }
        }

        public TimeSpan Duration
        {
            get 
            {
                DateTime startDate = Start ?? DateTime.MinValue;
                DateTime endDate = Start ?? DateTime.MaxValue;
                return (endDate - startDate); 
            }
        }

        public bool IsEmpty
        {
            get { return !(Start.HasValue || End.HasValue); }
        }

        #endregion        

        #region String Formatting
        private static readonly Regex QuarterDateRegEx = new Regex(@"[Qq](?<quarter>[1-4])\s?(?<year>[1-2]\d{3})", RegexOptions.Singleline);
        private static readonly Regex MonthDateRegEx = new Regex(@"(?<month>(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec))\s?(?<year>[1-2]\d{3})", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public void Parse(string value)
        {
            Start = null;
            End = null;            
            if (value.IsNullOrBlank())
            { return; }

            if (QuarterDateRegEx.IsMatch(value))
            {
                Match regExMatch = QuarterDateRegEx.Match(value);
                int year = Int32.Parse(regExMatch.Groups["year"].Value);
                int quarter = Int32.Parse(regExMatch.Groups["quarter"].Value);

                Start = new DateTime(year, 3 * (quarter - 1) + 1, 1);
                End = Start.Value.AddMonths(3);
                Kind = DateRangeKind.Quarter;

                return;
            }

            if (MonthDateRegEx.IsMatch(value))
            {
                Match regExMatch = MonthDateRegEx.Match(value);
                int year = Int32.Parse(regExMatch.Groups["year"].Value);
                string s = regExMatch.Groups["month"].Value;
                Month month = regExMatch.Groups["month"].Value.ParseEnum<Month>(Month.None);
                if (!month.IsDefined())
                { throw new FormatException(String.Format("Invalid Month '{0}'.", regExMatch.Groups["month"].Value)); }
                
                Start = new DateTime(year, (int) month, 1);
                End = Start.Value.AddMonths(1).AddDays(-1);
                Kind = DateRangeKind.Month;

                return;
            }

            Kind = value.ParseEnum<DateRangeKind>(DateRangeKind.Custom);
            if (Kind.IsCustom())
            {
                string[] dateRange = value.Split('-');
                if (dateRange.Length == 2)
                {
                    if (!dateRange[0].IsNullOrBlank())
                    {
                        DateTime startDate;
                        if (!DateTime.TryParse(dateRange[0], out startDate))
                        { throw new FormatException(String.Format("Invalid Date Format '{0}'.", dateRange[0])); }

                        Start = startDate;
                    }

                    if (!dateRange[1].IsNullOrBlank())
                    {
                        DateTime endDate;
                        if (!DateTime.TryParse(dateRange[1], out endDate))
                        { throw new FormatException(String.Format("Invalid Date Format '{0}'.", dateRange[1])); }

                        End = endDate;
                    }
                }
                else if (dateRange.Length == 1 && !dateRange[0].IsNullOrBlank())
                {
                    DateTime date;
                    if (!DateTime.TryParse(dateRange[0], out date))
                    { throw new FormatException(String.Format("Invalid Date Range Format '{0}'.", dateRange[0])); }

                    Start = date;
                    End = date;
                }
                else
                {
                    throw new FormatException(String.Format("Invalid Date Range Format '{0}'.", value));
                }
            }
            else
            {
                DateRange dateRange = DateRange.Create(Kind);
                Start = dateRange.Start;
                End = dateRange.End;
            }
        }

        public static DateRange ParseString(string value)
        {
            DateRange dateRange = new DateRange();
            dateRange.Parse(value);
            return dateRange;
        }

        public override string ToString()
        {
            return FormatString();
        }

        public string FormatString()
        {
            if (Kind.IsCustom())
            {
                return FormatValue();
            }
            else if (Kind.IsQuarter())
            {
                DateTime dt = Start.Value;
                return String.Format("Q{0}{1}", (dt.Month / 3) + 1, dt.Year);                
            }
            else if (Kind.IsMonth())
            {
                DateTime dt = Start.Value;
                return String.Format("{0:MMM yyyy}", dt);
            }
            else
            {
                return Kind.ToString();
            }            
        }

        public string FormatValue()
        {
            string dateStart = Start.HasValue ? Start.Value.ToString("d") : String.Empty;
            string dateEnd = End.HasValue ? End.Value.ToString("d") : String.Empty;
            return String.Format("{0}-{1}", dateStart, dateEnd);
        }

        #endregion

        #region Utility
        /// <summary>
        /// Returns the number of days between two dates based on a 360-day year (twelve 30-day months), which is used in some accounting calculations. 
        /// </summary>
        public static int Days360(DateTime startDate, DateTime endDate, bool european = false)
        {
            int days = 0;
            if (!european)
            {
                days = ((endDate.Year * 12 + endDate.Month) - (startDate.Year * 12 + startDate.Month)) * 30 + endDate.Day - startDate.Day;
            }
            else
            {
                days = ((endDate.Year * 12 + endDate.Month) - (startDate.Year * 12 + startDate.Month)) * 30 + Math.Min(endDate.Day, 30) - Math.Min(startDate.Day, 30);
            }

            return days;
        }

        public static TimeSpan BusinessDays(DateTime startDate, DateTime endDate)
        {
            List<DateTime> dates = new List<DateTime>();

            for (int i = 0; i < (int)(endDate - startDate).TotalDays; i++)
            { dates.Add(startDate.AddDays(i)); }

            int businessDays = dates.Count(date => !(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday));
            TimeSpan timeSpan = new TimeSpan(businessDays, 0, 0, 0);
            return timeSpan;
        }

        #endregion
    }
}
