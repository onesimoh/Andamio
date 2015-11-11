using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Diagnostics
{
    /// <summary>
    /// Interface for all Trace Event Log Recorders.
    /// </summary>
    public interface ILogRecorder
    {
        /// <summary>
        /// Logs the specified Log Entry.
        /// </summary>
        /// <param name="logEntry">The Log Entry to Trace.</param>
        void Trace(LogEntry logEntry);
    }
}
