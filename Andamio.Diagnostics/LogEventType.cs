using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Diagnostics
{
    public enum LogEventType
    {
        /// <summary>
        /// A very low level event that is only useful for tracing program execution.
        /// </summary>
        [EnumDisplay("Trace", Description = "A very low level event that is only useful for tracing program execution.")]
        Trace = 0,

        /// <summary>
        /// An important milestone or other non-error event.
        /// </summary>
        [EnumDisplay("Info", Description = "An important milestone or other non-error event.")]
        Information = 1,

        /// <summary>
        /// A potential problem with the current task.  No loss of data but possibly degraded service.
        /// </summary>
        [EnumDisplay("Warning", Description = "A potential problem with the current task.")]
        Warning = 2,

        /// <summary>
        /// A serious problem with the current task.  The task cannot recover automatically, but the program is still operable.
        /// </summary>
        [EnumDisplay("Error", Description = "A serious problem with the current task.")]
        Error = 3,

        /// <summary>
        /// A serious problem with the application.  The application may not be able to process any tasks until something intervenes.
        /// </summary>
        [EnumDisplay("Critical", Description = "A serious problem with the application.")]
        Critical = 4
    }
}
