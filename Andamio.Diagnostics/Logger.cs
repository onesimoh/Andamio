using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using Andamio;
using Andamio.Security;
using Andamio.Serialization;
using Andamio.Diagnostics.Listeners;
using Andamio.Diagnostics.Configuration;

namespace Andamio.Diagnostics
{
    /// <summary>
    /// Provide Tracing/Logging capabilities.
    /// </summary>
    public static partial class Logger
    {
        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        static Logger()
        {

        } 

        #endregion

        #region Debug
        /// <summary>
        /// Writes a formatted message to the configured Trace Source.
        /// </summary>
        /// <param name="message">A formatted message to output.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public static void Debug(string message, params object[] args)
        {
            string msg = String.Format(message, args);
            System.Diagnostics.Debug.WriteLine(msg);
        }

        /// <summary>
        /// Writes a formatted message to the configured Trace Source.
        /// </summary>
        /// <param name="message">A formatted message to output.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public static void Debug(string category, string message, params object[] args)
        {
            string msg = String.Format(message, args);
            System.Diagnostics.Debug.WriteLine(msg, category);
        }

        /// <summary>
        /// Writes an exception message to the configured Trace Source.
        /// </summary>        
        /// <param name="exception">The Exception to write to the Trace Source.</param>
        /// <param name="message">A message to write alongside the specified exception.</param>
        public static void Debug(Exception exception, string message = null)
        {
            if (!message.IsNullOrBlank())
            {
                Debug("Exception: {0}. Message: {1}", exception.Message, message);                
            }
            else
            {
                Debug(exception.Message);
            }
        }

        #endregion

        #region Write
        /// <summary>
        /// Writes trace information to the Log.
        /// </summary>
        /// <param name="logEntry">A log entry to log.</param>
        public static void Write(LogEntry logEntry)
        {
            if (logEntry == null) throw new ArgumentNullException("logEntry");
            Log logger = Log.FromConfig();
            logger.Listeners.ForEach(listener => listener.Trace(logEntry));
        }

        #endregion
    }
}
