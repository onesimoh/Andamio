using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Andamio;
using Andamio.Security;

namespace Andamio.Diagnostics
{
    public  partial class Log
    {
        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="message">A message to output.</param>
        public  void Info(string message)
        {
            Info(null, 0, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="message">A message to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public  void Info(string message, params LogAttachment[] attachments)
        {
            Info(null, 0, message, null, attachments);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="message">A message to output.</param>        
        public  void Info(string user, string message)
        {
            Info(user, 0, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>        
        public  void Info(int eventId, string message)
        {
            Info(null, eventId, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>        
        public  void Info(string user, int eventId, string message)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Information, message, eventId: eventId);
            Write(logEntry);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public  void Info(string message, Exception e)
        {
            Info(null, 0, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public  void Info(string user, string message, Exception e)
        {
            Info(user, 0, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public  void Info(int eventId, string message, Exception e)
        {
            Info(null, eventId, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public  void Info(string user, int eventId, string message, Exception e)
        {            
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Information, message, e, eventId: eventId);
            Write(logEntry);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public  void Info(string message, Exception e, params LogAttachment[] attachments)
        {
            Info(null, 0, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public  void Info(string user, string message, Exception e, params LogAttachment[] attachments)
        {
            Info(user, 0, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public  void Info(int eventId, string message, Exception e, params LogAttachment[] attachments)
        {
            Info(null, eventId, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="user">Represents the User that generated the Log Event.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public  void Info(string user, int eventId, string message, Exception e, params LogAttachment[] attachments)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Information, message, e, attachments, eventId);
            Write(logEntry);
        }
    }
}
