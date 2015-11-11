using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Andamio.Security;

namespace Andamio.Diagnostics
{
    /// <summary>
    /// Utility class to provide Tracing/Logging capabilities.
    /// </summary>
    public static partial class Logger
    {
        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="message">A message to output.</param>
        public static void Error(string message)
        {
            Error(null, 0, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="message">A message to output.</param>
        public static void Error(string user, string message)
        {
            Error(user, 0, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        public static void Error(int eventId, string message)
        {
            Error(null, eventId, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>        
        /// <param name="message">A message to output.</param>
        public static void Error(string user, int eventId, string message)
        {
            Log logger = Log.FromConfig();
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Error, message, eventId: eventId);
            logger.Listeners.ForEach(listener => listener.Trace(logEntry));
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public static void Error(string message, Exception e)
        {
            Error(null, 0, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public static void Error(string user, string message, Exception e)
        {
            Error(user, 0, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public static void Error(int eventId, string message, Exception e)
        {
            Error(null, eventId, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>       
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public static void Error(string user, int eventId, string message, Exception e)
        {
            Log logger = Log.FromConfig();
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Error, message, e, eventId: eventId);
            logger.Listeners.ForEach(listener => listener.Trace(logEntry));
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public static void Error(string message, Exception e, params LogAttachment[] attachments)
        {
            Error(null, 0, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public static void Error(string user, string message, Exception e, params LogAttachment[] attachments)
        {
            Error(user, 0, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public static void Error(int eventId, string message, Exception e, params LogAttachment[] attachments)
        {
            Error(null, eventId, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public static void Error(string user, int eventId, string message, Exception e, params LogAttachment[] attachments)
        {
            Log logger = Log.FromConfig();
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Error, message, e, attachments, eventId);
            logger.Listeners.ForEach(listener => listener.Trace(logEntry));
        }
    }
}
