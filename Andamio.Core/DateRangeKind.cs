using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio
{
    public enum DateRangeKind
    {
        [EnumDisplay("", IsSelectable = false)]
        None = 0,

        [EnumDisplay("Today")]
        Today = 1,

        [EnumDisplay("Last 7 Days")]
        Last7Days = 2,

        [EnumDisplay("Last 30 Days")]
        Last30Days = 3,

        [EnumDisplay("Last 90 Days")]
        Last90Days = 4,

        [EnumDisplay("Last 6 Months")]
        Last6Months = 5,

        [EnumDisplay("Last 12 Months")]
        Last12Months = 6,

        [EnumDisplay("Month To Date")]
        MonthToDate = 7,

        [EnumDisplay("Quarter To Date")]
        QuarterToDate = 8,

        [EnumDisplay("Year To Date")]
        YearToDate = 9,

        [EnumDisplay("Previous Month")]
        PreviousMonth = 10,

        [EnumDisplay("Previous Quarter")]
        PreviousQuarter = 11,

        [EnumDisplay("Previous Year")]
        PreviousYear = 12,

        [EnumDisplay("Quarter")]
        Quarter = 13,

        [EnumDisplay("Month")]
        Month = 14,

        [EnumDisplay("Custom")]
        Custom = 15,
    }

    public static class DateRangeKindExtensions
    {
        public static bool IsDefined(this DateRangeKind dateRangeKind)
        {
            return dateRangeKind != DateRangeKind.None;
        }
        public static bool IsCustom(this DateRangeKind dateRangeKind)
        {
            return dateRangeKind == DateRangeKind.Custom;
        }
        public static bool IsQuarter(this DateRangeKind dateRangeKind)
        {
            return dateRangeKind == DateRangeKind.Quarter;
        }
        public static bool IsMonth(this DateRangeKind dateRangeKind)
        {
            return dateRangeKind == DateRangeKind.Month;
        }
    }

}
