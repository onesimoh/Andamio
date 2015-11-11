using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;
using System.Runtime.Serialization;

using Andamio;
using Andamio.Serialization;
using Andamio.Collections;
using Andamio.Security;

namespace Andamio.Diagnostics
{
    /// <summary>
    /// Strongly Type Collection of Log Entries.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public class LogEntries : CollectionBase<LogEntry>
    {
        #region Constructors
        /// <summary>Default Constructor.</summary>
        public LogEntries() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the LogEntries with the specified entries.
        /// </summary>
        /// <param name="logEntries">The log entries to initialize the Collection.</param>
        public LogEntries(IEnumerable<LogEntry> logEntries) : base(logEntries)
        {
        }

        #endregion
    }


    /// <summary>
    /// Represents an entry for logging purposes.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{EventType}")]
    public sealed class LogEntry : ISerializable, IDynamicMarshaling
    {
        #region Constructors
        /// <summary>Default Constructor.</summary>
        internal LogEntry()
        {
        }

        /// <summary>
        /// Initializes a new Information LogEntry with the specified Message.
        /// </summary>
        /// <param name="message">The Message to initialize the LogEntry</param>
        public LogEntry(string message) : this(LogEventType.Information, message)
        {
        }

        /// <summary>
        /// Initializes a new LogEntry.
        /// </summary>
        /// <param name="eventType">The Trace Event Type.</param>
        /// <param name="message">The Log Message.</param>
        /// <param name="user">The user associated with the LogEntry.</param>
        /// <param name="exceptions">The Exceptions to log for the LogEntry.</param>
        /// <param name="attachments">The Attachments associated with the LogEntry.</param>
        public LogEntry(LogEventType eventType
            , string message
            , string user = null
            , IEnumerable<LogException> exceptions = null
            , IEnumerable<LogAttachment> attachments = null) : this()
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;

            EventType = eventType;
            EventId = null;
            Message = message;
            Owner = user;
            Attachments.AddRange(attachments);
            TimeStamp = DateTime.Now;
            Exceptions.AddRange(exceptions);
        }

        /// <summary>
        /// Initializes a new LogEntry.
        /// </summary>
        /// <param name="eventType">The Trace Event Type.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <param name="message">The Log Message.</param>
        /// <param name="user">The user associated with the LogEntry.</param>
        /// <param name="exceptions">The Exceptions to log for the LogEntry.</param>
        /// <param name="attachments">The Attachments associated with the LogEntry.</param>
        public LogEntry(LogEventType eventType
            , int eventId
            , string message
            , string user = null
            , IEnumerable<LogException> exceptions = null
            , IEnumerable<LogAttachment> attachments = null) : this()
        {
            EventType = eventType;
            EventId = eventId;
            Message = message;
            Owner = user;
            Attachments.AddRange(attachments);
            TimeStamp = DateTime.Now;
            Exceptions.AddRange(exceptions);
        }

        #endregion

        #region Create
        /// <summary>
        /// Creates a new Log Entry object.
        /// </summary>
        /// <param name="user">The user associated with the LogEntry.</param>
        /// <param name="eventType">The Trace Event Type.</param>
        /// <param name="message">The Log Message.</param>
        /// <param name="exception">The Exceptions to log for the LogEntry.</param>
        /// <param name="attachments">The Attachments associated with the LogEntry.</param>
        /// <param name="eventId">Represents an categorical Log Event.</param>
        /// <returns>An new LogEntry instance.</returns>
        public static LogEntry Create(string user
            , LogEventType eventType
            , string message            
            , Exception exception = null
            , IEnumerable<LogAttachment> attachments = null
            , int? eventId = null)
        {
            LogEntry logEntry = (eventId.HasValue) ? new LogEntry(eventType, eventId.Value, message) : new LogEntry(eventType, message);
            logEntry.TimeStamp = DateTime.UtcNow;

            if (exception != null)
            {
                logEntry.Exceptions.AddRange(LogException.Expand(exception).Select(e => new LogException(e)));
            }

            if (attachments != null && attachments.Any())
            { 
                logEntry.Attachments.AddRange(attachments); 
            }

            if (!user.IsNullOrBlank())
            { 
                logEntry.Owner = user; 
            }

            return logEntry;
        }

        /// <summary>
        /// Creates a new Log Entry object.
        /// </summary>
        /// <param name="eventType">The Trace Event Type.</param>
        /// <param name="message">The Log Message.</param>
        /// <param name="exception">The Exceptions to log for the LogEntry.</param>
        /// <returns>An new LogEntry instance.</returns>
        public static LogEntry Create(LogEventType eventType, string message, Exception exception = null)
        {
            return Create(UserIdentity.GetIdentityName(), eventType, message, exception);
        }

        /// <summary>
        /// Creates a new Warning Log Entry object.
        /// </summary>
        /// <param name="exception">The Exceptions to log for the LogEntry.</param>
        /// <param name="message">The Log Message.</param>
        /// <param name="user">The User associated with the Log Entry.</param>
        /// <returns>An new LogEntry instance.</returns>
        public static LogEntry Warning(Exception exception, string message = null, string user = null)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            return Create(user, LogEventType.Warning, message, exception);
        }

        /// <summary>
        /// Creates a new Warning Log Entry object.
        /// </summary>
        /// <param name="message">The Log Message.</param>
        /// <param name="user">The User associated with the Log Entry.</param>
        /// <returns>An new LogEntry instance.</returns>
        public static LogEntry Warning(string message, string user = null)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            return Create(user, LogEventType.Warning, message);
        }

        /// <summary>
        /// Creates a new Error Log Entry object.
        /// </summary>
        /// <param name="exception">The Exceptions to log for the LogEntry.</param>
        /// <param name="message">The Log Message.</param>
        /// <param name="user">The User associated with the Log Entry.</param>
        /// <returns>An new LogEntry instance.</returns>
        public static LogEntry Error(Exception exception, string message = null, string user = null)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            return Create(user, LogEventType.Error, message, exception);
        }

        /// <summary>
        /// Creates a new Critical Log Entry object.
        /// </summary>
        /// <param name="exception">The Exceptions to log for the LogEntry.</param>
        /// <param name="message">The Log Message.</param>
        /// <param name="user">The User associated with the Log Entry.</param>
        /// <returns>An new LogEntry instance.</returns>
        public static LogEntry Critical(Exception exception, string message = null, string user = null)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            return Create(user, LogEventType.Critical, message, exception);
        }

        /// <summary>
        /// Creates a new Info Log Entry object.
        /// </summary>
        /// <param name="message">The Log Message.</param>
        /// <param name="user">The User associated with the Log Entry.</param>
        /// <returns>An new LogEntry instance.</returns>
        public static LogEntry Info(string message, string user = null)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            return Create(user, LogEventType.Information, message);
        }

        /// <summary>
        /// Creates a new Verbose Log Entry object.
        /// </summary>
        /// <param name="message">The Log Message.</param>
        /// <param name="user">The User associated with the Log Entry.</param>
        /// <returns>An new LogEntry instance.</returns>
        public static LogEntry DebugTrace(string message, string user = null)
        {
            user = user.IsNullOrBlank() ? UserIdentity.GetIdentityName() : user;
            return Create(user, LogEventType.Trace, message);
        }

        #endregion

        #region LogEventType
        /// <summary>
        /// Gets or Sets the TraceEventType.
        /// </summary>
        public LogEventType EventType { get; set; }

        #endregion

        #region Event
        /// <summary>
        /// Gets or Sets the EventId.
        /// </summary>
        public int? EventId { get; set; }
        
        /// <summary>
        /// True if an Event Id has been assigned; False otherwise.
        /// </summary>
        public bool HasEventId
        {
            get { return EventId.HasValue; }
        }

        #endregion

        #region Message
        /// <summary>
        /// Gets or Sets the Message.
        /// </summary>
        public string Message { get; internal set; }

        #endregion

        #region Exception
        private readonly LogExceptions _exceptions = new LogExceptions();
        /// <summary>
        /// Contains a strongly typed Collection of Log Exception objects.
        /// </summary>
        public LogExceptions Exceptions 
        {
            get { return _exceptions; }
        }

        #endregion

        #region User
        /// <summary>
        /// Gets or Sets the User associated with this Log Entry.
        /// </summary>
        public string Owner { get; set; }

        #endregion

        #region Time Stamp
        /// <summary>
        /// Gets or Sets the Date/Time when the Log Entry was created.
        /// </summary>
        public DateTime TimeStamp { get; set; }
        #endregion

        #region Attachements
        private readonly LogAttachments _attachments = new LogAttachments();
        /// <summary>
        /// Contains a strongly typed Collection of Log Attachments objects.
        /// </summary>
        public LogAttachments Attachments
        {
            get { return _attachments; }
        }

        #endregion

        #region Serialization
        /// <summary>
        /// Generates a new Xml Element with the information of Log Entry.
        /// </summary>
        /// <param name="name">The name for the Xml Element.</param>
        /// <returns>a New XElement.</returns>
        public XElement GenerateXml(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name);

            element.Add(new XAttribute("Type", EventType.ToString()));            
            element.Add(new XAttribute("Owner", Owner));
            element.Add(new XAttribute("Time", TimeStamp));
            element.Add(new XElement(xmlns + "Message", Message));

            if (HasEventId)
            { element.Add(new XAttribute("EventId", EventId.Value)); }

            var exceptions = Exceptions.Select(e => e.GenerateXElement(xmlns + "Exception"));
            if (exceptions.Any())
            {
                if (exceptions.Count() == 1)
                {
                    element.Add(exceptions.First());
                }
                else
                {
                    element.Add(new XElement(xmlns + "Exceptions", exceptions));
                }
            }

            if (Attachments.Any())
            {
                element.Add(Attachments.GenerateXml(xmlns + "Attachments"));
            }

            return element;
        }

        /// <summary>
        /// Creates a LogEntry object from the specified Xml Element.
        /// </summary>
        /// <param name="element">The Xml Element that contains the Log Entry information.</param>
        /// <returns>A LogEntry object.</returns>
        internal static LogEntry CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            LogEntry logEntry = new LogEntry()
            {
                EventType = element.Attribute("Type").Enum<LogEventType>(LogEventType.Information),
                Owner = element.Attribute("Owner").Optional(),
                EventId =  element.Attribute("EventId").NullableInt32(),
                TimeStamp = element.Attribute("Time").DateTime(),
            };

            string message;
            if (element.Element(xmlns + "Message").TryGetValue(out message))
            {
                logEntry.Message = message;
            }

            var exceptions = element.Descendants(xmlns + "Exception");
            if (exceptions.Any())
            {
                logEntry.Exceptions.AddRange(exceptions.Select(e => LogException.CreateFromXElement(e)));
            }

            return logEntry;
        }

        public static LogEntry FromBinaryContent(BinaryContent binaryContent)
        {
            if (binaryContent == null)
            { throw new ArgumentNullException("binaryContent"); }

            return binaryContent.Deserialize<LogEntry>();
        }

        /// <summary>
        /// Creates a new instance and populates the object from the specified serialized data.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that contains the information to populate the instance.</param>
        /// <param name="context">The source for this serialization.<param>
        public LogEntry(SerializationInfo info, StreamingContext context)
        {
            EventType = info.GetEnum<LogEventType>("Type");            
            Owner = info.GetString("Owner");
            EventId =  info.GetNullable<Int32>("EventId");
            TimeStamp = info.GetDateTime("TimeStamp");
            Message = info.GetString("Message");

            var exceptions = info.GetSerializableArray<LogException>("Exceptions");
            if (exceptions != null && exceptions.Any())
            {  Exceptions.AddRange(exceptions);  }
        }

        /// <summary>
        //  Populates a System.Runtime.Serialization.SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination (see System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddEnum<LogEventType>("Type", EventType);
            info.AddValue("Owner", Owner);
            info.AddValue("EventId", EventId);
            info.AddValue("TimeStamp", TimeStamp);
            info.AddValue("Message", Message);
            info.AddSerializableArray("Exceptions", Exceptions.ToArray());
        }

        /// <summary>
        //  Populates a System.Runtime.Serialization.SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        object IDynamicMarshaling.Write()
        {
            return new
            {
                EventType = EventType.DisplayName(),
                Owner = Owner,
                EventId = EventId,
                TimeStamp = TimeStamp,
                Message = Message,
                Exceptions = Exceptions.Select(e => e.Dynamic()),
                Attachments = Attachments.Select(attachment => new { attachment.Key, attachment.Value }),
            };
        }


        #endregion
    }
}
