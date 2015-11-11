using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Runtime.Serialization;

using Andamio;
using Andamio.Collections;
using Andamio.Serialization;

namespace Andamio.Diagnostics
{
    /// <summary>
    /// Strongly Type Collection of Log Exceptions entries.
    /// </summary>
    [Serializable]
    public class LogExceptions : CollectionBase<LogException>
    {
        #region Constructors
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public LogExceptions() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance that contains the specified entries.
        /// </summary>
        /// <param name="logExceptions">The Log Exception entries to initialize the Collection</param>
        public LogExceptions(IEnumerable<LogException> logExceptions)
            : base(logExceptions)
        {

        }

        #endregion
    }


    /// <summary>
    /// Wraps an Exception for logging purposes.
    /// </summary>
    [Serializable]
    [XmlRoot("LogException")]
    public sealed class LogException : ISerializable, IDynamicMarshaling
    {
        #region Constructors
        /// <summary>Default Constructor.</summary>
        private LogException()
        {
        }

        /// <summary>
        /// Initializes a new Instance that contains the information from the specified Exception object.
        /// </summary>
        /// <param name="exception">The Exception object to initialize the LogException instance.</param>
        public LogException(Exception exception)
        {
            if (exception == null)
            { throw new ArgumentNullException("exception"); }

            Message = exception.Message;
            StackTrace = exception.StackTrace;
            Source = exception.Source;
            ExceptionType = exception.GetType().ToString();
        }

        #endregion

        #region Conversion
        /// <summary>
        /// Converts the specified Exception object to a LogException.
        /// </summary>
        /// <param name="exception">The Exception object.</param>
        /// <returns>An instance of LogException that contains the Data from the specified Exception object.</returns>
        public static implicit operator LogException(Exception exception)
        {
            return new LogException(exception);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the Exception Message.
        /// </summary>
        [XmlElement("Message")]
        public string Message { get; private set; }

        /// <summary>
        /// Gets the Exception Stack Trace.
        /// </summary>
        [XmlElement("StackTrace")]
        public string StackTrace { get; private set; }

        /// <summary>
        /// Gets the Exception Source.
        /// </summary>
        [XmlElement("Source")]
        public string Source { get; private set; }

        /// <summary>
        /// Gets the Exception Type.
        /// </summary>
        [XmlElement("ExceptionType")]
        public string ExceptionType { get; private set; }

        #endregion

        #region Expand
        /// <summary>
        /// Expands the chain of inner Exceptions into an Array.
        /// </summary>
        /// <param name="exception">The Exception object to expand.</param>
        /// <returns>An Array of Exception objects.</returns>
        public static Exception[] Expand(Exception exception)
        {
            if (exception == null)
            { throw new ArgumentNullException("exception"); }

            List<Exception> exceptions = new List<Exception>();
            exceptions.Add(exception);

            Exception childException = exception.InnerException;
            while (childException != null)
            {
                exceptions.Add(childException);
                childException = childException.InnerException;
            }

            return exceptions.ToArray();
        }

        #endregion

        #region Serialization
        /// <summary>
        /// Generates a new Xml Element with the information of Log Exception.
        /// </summary>
        /// <param name="name">The name for the Xml Element.</param>
        /// <returns>a New XElement.</returns>
        public XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name);

            element.Add(new XElement(xmlns + "Message", Message));
            if (!StackTrace.IsNullOrBlank())
            {
                element.Add(new XElement(xmlns + "StackTrace", StackTrace.Trim()));
            }
            element.Add(new XElement(xmlns + "Source", Source));
            element.Add(new XElement(xmlns + "ExceptionType", ExceptionType));

            return element;
        }

        /// <summary>
        /// Creates a LogException object from the specified Xml Element.
        /// </summary>
        /// <param name="element">The Xml Element that contains the Log Exception information.</param>
        /// <returns>A LogException object.</returns>
        public static LogException CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            LogException logException = new LogException()
            {
                Message = element.Element(xmlns + "Message").Value,
                Source = element.Element(xmlns + "Source").Value,
                ExceptionType = element.Element(xmlns + "ExceptionType").Value,
            };

            string stackTrace;
            if (element.Element(xmlns + "StackTrace").TryGetValue(out stackTrace))
            {
                logException.StackTrace = stackTrace;
            }

            return logException;
        }

        /// <summary>
        /// Creates a new instance and populates the object from the specified serialized data.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that contains the information to populate the instance.</param>
        /// <param name="context">The source for this serialization.<param>
        public LogException(SerializationInfo info, StreamingContext context)
        {
            Message = info.GetString("Message");
            Source = info.GetString("Source");
            ExceptionType = info.GetString("ExceptionType");
            StackTrace = info.GetString("StackTrace"); 
        }

        /// <summary>
        //  Populates a System.Runtime.Serialization.SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination (see System.Runtime.Serialization.StreamingContext) for this serialization.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Message", Message);
            info.AddValue("Source", Source);
            info.AddValue("ExceptionType", ExceptionType);
            info.AddValue("StackTrace", StackTrace);
        }
        
        object IDynamicMarshaling.Write()
        {
            return new
            {
                Message = Message,
                Source = Source,
                ExceptionType = ExceptionType,
                StackTrace = StackTrace,
            };
        }


        #endregion

        #region Overrides
        /// <summary>
        /// Returns the Stack Trace for the Log Exception.
        /// </summary>
        /// <returns>A String object,</returns>
        public override string ToString()
        {
            return this.StackTrace;
        }
        #endregion
    }
}
