using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Andamio;
using Andamio.Diagnostics.Sqllite;

namespace Andamio.Diagnostics.Listeners
{
    #region Settings
    /// <summary>
    /// Defines the settings for the Sqlite Trace Listener.
    /// </summary>
    public sealed class SqliteListenerSettings : TraceListenerSettings
    {
        /// <summary>Default Constructor</summary>
        private SqliteListenerSettings() : base()
        {
        }

        /// <summary>Default Constructor</summary>
        public SqliteListenerSettings(string filePath) : this()
        {
            if (filePath.IsNullOrBlank()) throw new ArgumentNullException("filePath");
            FilePath = filePath;
        }

        /// <summary>
        /// Gets or Sets the File Path for the Log File.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Creates the Xml listener from the current Settings, if a Listener Type is not provided then an instance of XmlTraceListener is returned.
        /// </summary>
        /// <returns>An Xml Log Recorder Listener instance.</returns>
        public override ILogRecorder CreateListener()
        {
            ILogRecorder listener =  new SqliteListener(this);
            return listener;
        }
    }

    #endregion


    public class SqliteListener : TraceListenerBase
    {
        #region Constructors
        /// <summary>Default Constructor</summary>
        private SqliteListener()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instace of the DatabaseTraceListener class.
        /// </summary>
        /// <param name="settings">The settings for the DatabaseTraceListener instance.</param>
        public SqliteListener(SqliteListenerSettings settings)
            : this()
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (settings.FilePath.IsNullOrBlank()) throw new ArgumentException("FilePath was not provided.", "settings");
            Settings = settings;
        }

        #endregion

        #region Settings
        /// <summary>
        /// Gets the settings for the DatabaseTraceListener.
        /// </summary>
        public SqliteListenerSettings Settings { get; private set; }

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
            using (SqliteDataContext context = new SqliteDataContext(Settings.FilePath))
            {
                context.LogEvents.Add(logEntry);
                context.SaveChanges();
            }
        }

        #endregion
    }
}

