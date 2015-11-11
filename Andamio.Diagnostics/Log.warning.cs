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
        /// <param name="message">A message to output.</param>
        public void Warning(string message)
        {
            Warning(null, 0, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="message">A message to output.</param>
        public void Warning(string user, string message)
        {
            Warning(user, 0, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        public void Warning(int eventId, string message)
        {
            Warning(null, eventId, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>        
        /// <param name="message">A message to output.</param>
        public void Warning(string user, int eventId, string message)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Warning, message, eventId: eventId);
            Write(logEntry);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public void Warning(string message, Exception e)
        {
            Warning(null, 0, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public void Warning(string user, string message, Exception e)
        {
            Warning(user, 0, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public void Warning(int eventId, string message, Exception e)
        {
            Warning(null, eventId, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>       
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public void Warning(string user, int eventId, string message, Exception e)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Warning, message, e, eventId: eventId);
            Write(logEntry);            
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public void Warning(string message, Exception e, params LogAttachment[] attachments)
        {
            Warning(null, 0, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public void Warning(string user, string message, Exception e, params LogAttachment[] attachments)
        {
            Warning(user, 0, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public void Warning(int eventId, string message, Exception e, params LogAttachment[] attachments)
        {
            Warning(null, eventId, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public void Warning(string user, int eventId, string message, Exception e, params LogAttachment[] attachments)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Warning, message, e, attachments, eventId);
            Write(logEntry);
        }
    }
}
