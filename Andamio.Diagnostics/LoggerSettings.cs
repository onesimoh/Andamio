using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Andamio;
using Andamio.Collections;
using Andamio.Security;
using Andamio.Diagnostics.Listeners;
using Andamio.Diagnostics.Configuration;

namespace Andamio.Diagnostics
{
    /// <summary>
    /// Contains the Logger Settings to initialize the logger.
    /// </summary>
    public class LoggerSettings : Disposable
    {
        #region Constructors
        /// <summary>Default Constructor.</summary>
        private LoggerSettings()
        {
        }

        #endregion

        #region Settings
        /// <summary>
        /// Contains the Xml Trace Listener Settings.
        /// </summary>
        public XmlTraceListenerSettings XmlTraceListener { get; set; }

        /// <summary>
        /// Contains the Database Trace Listener Settings.
        /// </summary>
        public SqliteListenerSettings SqliteTraceListener { get; set; }

        /// <summary>
        /// Contains the Xml Trace Listener Settings.
        /// </summary>
        public ConsoleListenerSettings ConsoleListener { get; set; }

        /// <summary>
        /// Contains the Database Trace Listener Settings.
        /// </summary>
        public DebugListenerSettings DebugListener { get; set; }

        /// <summary>
        /// Loads the settings defined in the Configuration.
        /// </summary>
        public static LoggerSettings FromConfiguration()
        {
            LoggerSettings loggerSettings = new LoggerSettings();
            DiagnosisConfigSection diagnosisConfig = DiagnosisConfigSection.FromConfig();

            // Xml Log Settings
            XmlLogSettingsElement xmlLogSettings = diagnosisConfig.XmlLogSettings;
            if (xmlLogSettings != null && xmlLogSettings.Enabled)
            {
                loggerSettings.XmlTraceListener = new XmlTraceListenerSettings()
                {
                    MaxSizeInMB = xmlLogSettings.MaxSizeInMB,
                    Mode = xmlLogSettings.Mode,
                    FilePath = xmlLogSettings.FilePath,
                    Listener = xmlLogSettings.Listener,
                    EventLogName = xmlLogSettings.EventLogName,
                    TransformationFile = xmlLogSettings.TransformationFile,
                };
            }

            // Database Log Settings
            SqliteSettingsElement sqliteSettings = diagnosisConfig.SqliteSettings;
            if (sqliteSettings != null && sqliteSettings.Enabled)
            {
                loggerSettings.SqliteTraceListener = new SqliteListenerSettings(sqliteSettings.FilePath)
                {
                    Listener = sqliteSettings.Listener,
                    EventLogName = sqliteSettings.EventLogName,
                };
            }

            // Console Log Settings
            ConsoleSettingsElement consoleSettings = diagnosisConfig.ConsoleLogSettings;
            if (consoleSettings != null && consoleSettings.Enabled)
            {
                loggerSettings.ConsoleListener = new ConsoleListenerSettings()
                {
                    Listener = consoleSettings.Listener,
                    EventLogName = consoleSettings.EventLogName,
                };
            }

            // Debug Log Settings
            DebugSettingsElement debugSettings = diagnosisConfig.DebugLogSettings;
            if (debugSettings != null && debugSettings.Enabled)
            {
                loggerSettings.DebugListener = new DebugListenerSettings()
                {
                    Listener = debugSettings.Listener,
                    EventLogName = debugSettings.EventLogName,
                };
            }

            return loggerSettings;
        }

        #endregion

        #region Disposable
        /// <summary>
        /// Releases all resources associated with this object.
        /// </summary>
        protected override void Cleanup()
        {
        }

        #endregion
    }
}