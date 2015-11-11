using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Andamio.Diagnostics.Listeners
{
    #region Settings
    public class DebugListenerSettings : TraceListenerSettings
    {
        /// <summary>Default Constructor</summary>
        public DebugListenerSettings() : base()
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
            { listener = new DebugTraceListener(this); }
            return listener;
        }    
    }

    #endregion


    /// <summary>
    /// Debug Trace Listener.
    /// </summary>
    public class DebugTraceListener : TraceListenerBase
    {
        #region Constructors
        /// <summary>Default Constructor</summary>
        public DebugTraceListener() : base()
        {
        }


        /// <summary>
        /// Creates a new instace of the DebugTraceListener class.
        /// </summary>
        /// <param name="settings">The settings for the Debug Trace Listener instance.</param>
        public DebugTraceListener(DebugListenerSettings settings) : this()
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;
        }

        #endregion

        #region Settings
        /// <summary>
        /// Gets the settings for the Xml Trace Listener.
        /// </summary>
        public DebugListenerSettings Settings { get; private set; }

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
            Debug.WriteLine("{0}: {1}", logEntry.EventType.DisplayName(), logEntry.Message);
            if (logEntry.Exceptions.Any())
            {
                logEntry.Exceptions.ForEach(delegate(LogException exception)
                {
                    Debug.WriteLine("{0}: {1}", exception.ExceptionType, exception.Message);
                    Debug.WriteLine(exception.StackTrace);
                });                
            }

        }

        #endregion
    }
}
