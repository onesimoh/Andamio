using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Andamio;
using Andamio.Configuration;
using Andamio.Data;
using Andamio.Data.Access;
using Andamio.Data.Entities;
using Andamio.Data.Transactions;
using Andamio.Data.Serialization;
using Andamio.Diagnostics;

namespace Andamio.Diagnostics.Sqllite
{
    #region Collection
    public partial class LogEvents
    {
        #region Conversion
        public static implicit operator LogEvents(LogEntries logEntries)
        {
            return new LogEvents(logEntries.Select(logEntry => (LogEvent) logEntry));
        }

        public static implicit operator LogEntries(LogEvents logEvents)
        {
            return new LogEntries(logEvents.Select(logEvent => (LogEntry) logEvent));
        }

        #endregion
    }

    #endregion


    [DebuggerDisplay("{Type}")]
	public partial class LogEvent
	{
        #region Conversion
        public static implicit operator LogEvent(LogEntry logEntry)
        {
            if (logEntry == null) throw new ArgumentNullException("logEntry");
            return new LogEvent() 
            {
                Type = logEntry.EventType,
                Message = logEntry.Message,
                Owner = logEntry.Owner,
                Content = logEntry.ToBinaryContent(),
                TimeStamp = logEntry.TimeStamp.FormatDateTime(),
            };
        }

        public static implicit operator LogEntry(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException("logEvent");
            BinaryContent binaryContent = logEvent.Content;
            return binaryContent.Deserialize<LogEntry>();
        }

        #endregion
    }
}