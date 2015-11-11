using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;

using Andamio;

namespace Andamio.Diagnostics.Listeners
{
    /// <summary>
    /// Provides Base functionality for all Trace Listeners.
    /// </summary>
    public abstract class TraceListenerBase :  Disposable, ILogRecorder
    {
        #region Constructors, Destructor
        /// <summary>
        /// Default Constructors.
        /// </summary>
        public TraceListenerBase() : base()
        {
            Enabled = true;
        }

        #endregion

        #region Enabled
        /// <summary>
        /// Gets or Sets whether Trace Listener is Enabled or not.
        /// </summary>
        public bool Enabled { get; set; }

        #endregion

        #region Trace
        /// <summary>
        /// One Byte: 1048576
        /// </summary>
        protected const long BYTE = 1048576;

        /// <summary>
        /// Executes before the Log Entry is logged.
        /// </summary>
        protected abstract void PreTrace();

        /// <summary>
        /// Executes after the Log Entry is logged.
        /// </summary>
        protected abstract void PostTrace();

        /// <summary>
        /// Logs the specified Log Entry.
        /// </summary>
        /// <param name="logEntry">The Log Entry to write to the Log.</param>
        protected abstract void WriteLogEntry(LogEntry logEntry);

        private object _syncLock = new object();
        /// <summary>
        /// Writes the specified Log Entry to the Log.
        /// </summary>
        /// <param name="logEntry">The Log Entry to write to the Log.</param>
        public virtual void Trace(LogEntry logEntry)
        {
            lock (_syncLock)
            {
                try
                {
                    PreTrace();
                    WriteLogEntry(logEntry);
                    PostTrace();
                }
                catch (Exception e)
                {
                    Logger.Debug(e);
                }
            }
        }

        #endregion

        #region Disposable Members
        /// <summary>
        /// Provides functionality to clean up or destroy any resources used by the Trace Listener when disposed.
        /// </summary>
        protected override void Cleanup()
        {
        }

        #endregion
    }
}
