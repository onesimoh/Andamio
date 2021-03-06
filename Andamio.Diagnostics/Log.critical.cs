﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Andamio.Security;

namespace Andamio.Diagnostics
{
    public partial class Log
    {
        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="message">A message to output.</param>
        public  void Critrical(string message)
        {
            Critrical(null, 0, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="message">A message to output.</param>
        public  void Critrical(string user, string message)
        {
            Critrical(user, 0, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>        
        public  void Critrical(int eventId, string message)
        {
            Critrical(null, eventId, message);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>        
        public  void Critrical(string user, int eventId, string message)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Critical, message, eventId: eventId);
            Write(logEntry);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public  void Critrical(string message, Exception e)
        {
            Critrical(null, 0, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public  void Critrical(string user, string message, Exception e)
        {
            Critrical(user, 0, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public  void Critrical(int eventId, string message, Exception e)
        {
            Critrical(null, eventId, message, e);
        }

        /// <summary>
        /// Writes event information, a message, and exception ot configured TraceSource.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        public  void Critrical(string user, int eventId, string message, Exception e)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Critical, message, e, eventId: eventId);
            Write(logEntry);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public  void Critrical(string message, Exception e, params LogAttachment[] attachments)
        {
            Critrical(null, 0, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public  void Critrical(string user, string message, Exception e, params LogAttachment[] attachments)
        {
            Critrical(user, 0, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public  void Critrical(int eventId, string message, Exception e, params LogAttachment[] attachments)
        {
            Critrical(null, eventId, message, e, attachments);
        }

        /// <summary>
        /// Writes a message, exception, and a list of KeyValuePair&lt;string, object&gt; to the configured Trace Source.
        /// </summary>
        /// <param name="user">Represents the User that generated the Exception.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">A message to output.</param>
        /// <param name="e">An exception to output.</param>
        /// <param name="attachments">The list of KeyValuePair&lt;string, object&gt; to output.</param>
        public  void Critrical(string user, int eventId, string message, Exception e, params LogAttachment[] attachments)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            LogEntry logEntry = LogEntry.Create(user, LogEventType.Critical, message, e, attachments, eventId);
            Write(logEntry);
        }
    }
}
