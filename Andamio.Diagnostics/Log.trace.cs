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
    public partial class Log
    {
        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="message">A message to output.</param>
        public void Trace(string message)
        {
            Trace(null, 0, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="message">A message to output.</param>
        public void Trace(string user, string message)
        {
            Trace(user, 0, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        public void Trace(int eventId, string message)
        {
            Trace(null, eventId, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        public void Trace(string user, int eventId, string message)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Trace, message, eventId: eventId);
            Write(logEntry);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public void Trace(string message, Exception e)
        {
            Trace(null, 0, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public void Trace(string user, string message, Exception e)
        {
            Trace(user, 0, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public void Trace(int eventId, string message, Exception e)
        {
            Trace(null, eventId, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public void Trace(string user, int eventId, string message, Exception e)
        {
            Log logger = Log.FromConfig();
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Trace, message, e, eventId: eventId);
            logger.Listeners.ForEach(listener => listener.Trace(logEntry));
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public void Trace(string message, Exception e, params LogAttachment[] attachments)
        {
            Trace(null, 0, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public void Trace(string user, string message, Exception e, params LogAttachment[] attachments)
        {
            Trace(user, 0, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public void Trace(int eventId, string message, Exception e, params LogAttachment[] attachments)
        {
            Trace(null, eventId, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="eventType">One of the LogEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public void Trace(string user, int eventId, string message, Exception e, params LogAttachment[] attachments)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Trace, message, e, attachments, eventId);
            Write(logEntry);
        }
    }
}
