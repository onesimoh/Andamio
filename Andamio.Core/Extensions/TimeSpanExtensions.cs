using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio
{
    /// <summary>
    /// Extension methods for System.TimeSpan.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Calculates a string representation of a TimeSpan object which is scaled to a unit that
        /// is meaningful to the reader.  The value is rounded.
        /// </summary>
        /// <remarks>
        /// Examples of how values will be scaled:
        /// < 100 ns (0 ticks)
        /// 100 ns to 999 ns
        /// 1.0 μs to 9.9 μs
        /// 10 μs to 999 μs
        /// 1.0 ms to 9.9 ms
        /// 10 ms to 999 ms
        /// 1.0 sec to 9.9 sec
        /// 10 sec to 119 sec
        /// 2.0 min to 9.9 min
        /// 10 min to 119 min
        /// 2.0 hrs to 9.9 hrs
        /// 10 hrs to 47 hrs
        /// 1.0 days to 9.9 days
        /// 10 days to 364 days
        /// 1.0 years to 9.9 years
        /// 10 years to 29,227 years (Int64.MaxValue ticks)
        /// </remarks>
        /// <param name="duration">The value to convert.</param>
        /// <returns>A string approximating the TimeSpan value.</returns>
        public static string ToScaledString(this TimeSpan duration)
        {
            duration = new TimeSpan(Math.Abs(duration.Ticks));

            if (duration.Ticks == 0)
            { return "< 100 ns"; }
            else if (duration.TotalMilliseconds < .001)
            { return String.Format("{0:N0} ns", Math.Round(duration.TotalMilliseconds * 1000000)); }
            else if (duration.TotalMilliseconds < .01)
            { return String.Format("{0:N1} μs", Math.Round(duration.TotalMilliseconds * 1000, 1)); }
            else if (duration.TotalMilliseconds < 1)
            { return String.Format("{0:N0} μs", Math.Round(duration.TotalMilliseconds * 1000)); }
            else if (duration.TotalMilliseconds < 10)
            { return String.Format("{0:N1} ms", Math.Round(duration.TotalMilliseconds, 1)); }
            else if (duration.TotalSeconds < 1)
            { return String.Format("{0:N0} ms", Math.Round(duration.TotalMilliseconds)); }
            else if (duration.TotalSeconds < 10)
            { return String.Format("{0:N1} sec", Math.Round(duration.TotalSeconds, 1)); }
            else if (duration.TotalMinutes < 2)
            { return String.Format("{0:N0} sec", Math.Round(duration.TotalSeconds)); }
            else if (duration.TotalMinutes < 10)
            { return String.Format("{0:N1} min", Math.Round(duration.TotalMinutes, 1)); }
            else if (duration.TotalHours < 2)
            { return String.Format("{0:N0} min", Math.Round(duration.TotalMinutes)); }
            else if (duration.TotalHours < 10)
            { return String.Format("{0:N1} hrs", Math.Round(duration.TotalHours, 1)); }
            else if (duration.TotalDays < 2)
            { return String.Format("{0:N0} hrs", Math.Round(duration.TotalHours)); }
            else if (duration.TotalDays < 10)
            { return String.Format("{0:N0} days", Math.Round(duration.TotalDays, 1)); }
            else if (duration.TotalDays < 365)
            { return String.Format("{0:N0} days", Math.Round(duration.TotalDays)); }
            else if (duration.TotalDays < 3650)
            { return String.Format("{0:N1} years", Math.Round(duration.TotalDays / 365.25, 1)); } //an AVERAGE year is 365.25 days
            else
            { return String.Format("{0:N0} years", Math.Round(duration.TotalDays / 365.25)); } //an AVERAGE year is 365.25 days            
        }

        /// <summary>
        /// Calculates a string representation of a TimeSpan object which is scaled to a unit that
        /// is meaningful to the reader.  The value is rounded.  
        /// 
        /// The difference between this conversion and ToScaledString() is that a SqlDateTime structure is only 
        /// accurate to 3.33 ms whereas a .Net DateTime structure is accurate to 100 ns.
        /// </summary>
        /// <remarks>
        /// Examples of how values will be scaled:
        /// < 3.33 ms (0 ticks)
        /// otherwise, same as ToScaledString()
        /// </remarks>
        /// <param name="duration">The value to convert.</param>
        /// <returns>A string approximating the TimeSpan value.</returns>
        public static string ToSqlScaledString(this TimeSpan duration)
        {
            //3.33 ms is the accuracy of the SqlDateTime structure
            if (duration.TotalMilliseconds < 3.33)
            { return "< 3.33 ms"; }
            else 
            { return duration.ToScaledString(); }          
        } 
    }
}
