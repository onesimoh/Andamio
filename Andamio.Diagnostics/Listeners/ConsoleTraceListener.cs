using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Andamio.Diagnostics.Listeners
{
    #region Settings
    public class ConsoleListenerSettings : TraceListenerSettings
    {
        /// <summary>Default Constructor</summary>
        public ConsoleListenerSettings()
            : base()
        {
        }

        /// <summary>
        /// Creates the Console trace listener from the current Settings, if a Listener Type is not provided then an instance of ConsoleTraceListener is returned.
        /// </summary>
        /// <returns>An Xml Log Recorder Listener instance.</returns>
        public override ILogRecorder CreateListener()
        {
            ILogRecorder listener = base.CreateListener();
            if (listener == null)
            { listener = new ConsoleTraceListener(this); }
            return listener;
        }
    }

    #endregion


    /// <summary>
    /// Debug Trace Listener.
    /// </summary>
    public class ConsoleTraceListener : TraceListenerBase
    {
        #region Constructors
        /// <summary>Default Constructor</summary>
        public ConsoleTraceListener()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instace of the ConsoleTraceListener class.
        /// </summary>
        /// <param name="settings">The settings for the ConsoleTraceListener instance.</param>
        public ConsoleTraceListener(ConsoleListenerSettings settings) : this()
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;

        }

        #endregion

        #region Settings
        /// <summary>
        /// Gets the settings for the Xml Trace Listener.
        /// </summary>
        public ConsoleListenerSettings Settings { get; private set; }

        #endregion

        #region Trace
        /// <summary>
        /// Executes before the Log Entry is logged.
        /// </summary>
        protected override void PreTrace()
        {                     
        }

        /// <summary>
        /// Executes after the Log Entry is logged.
        /// </summary>
        protected override void PostTrace()
        {
        }

        /// <summary>
        /// Logs the specified Log Entry.
        /// </summary>
        /// <param name="logEntry">The Log Entry to write to the Log.</param>
        protected override void WriteLogEntry(LogEntry logEntry)
        {
            if (logEntry == null) throw new ArgumentNullException("logEntry");
            Console.WriteLine("{0}: {1}", logEntry.EventType.DisplayName(), logEntry.Message);
            if (logEntry.Exceptions.Any())
            {
                logEntry.Exceptions.ForEach(delegate(LogException exception)
                {
                    Console.WriteLine("{0}: {1}", exception.ExceptionType, exception.Message);
                    Console.WriteLine(exception.StackTrace);
                });                
            }

        }

        #endregion
    }
}
