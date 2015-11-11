using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Andamio.Diagnostics.Listeners;

namespace Andamio.Diagnostics.Configuration
{
    /// <summary>
    /// Base class for Diagnosis Settings Configuration.
    /// </summary>
    public abstract class LogSettingsElementBase : ConfigurationElement
    {
        #region Private Fields, Constants
        private const string EnabledProperty = "enabled";
        private const string ListenerProperty = "listener";
        private const string EventLogNameProperty = "eventLogName";
        #endregion

        #region Constructor
        /// <summary>Default Constructor</summary>
        public LogSettingsElementBase()
            : base()
        {
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or Sets the custom Listener.
        /// </summary>
        [ConfigurationProperty(ListenerProperty, IsRequired = false)]
        public string Listener
        {
            get { return (string)this[ListenerProperty]; }
            set { this[ListenerProperty] = value; }
        }

        /// <summary>
        /// Gets or Sets whether Xml Logging is Enabled.
        /// </summary>
        [ConfigurationProperty(EnabledProperty, IsRequired = false, DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)this[EnabledProperty]; }
            set { this[EnabledProperty] = value; }
        }

        /// <summary>
        /// Gets or Sets the Event Log Name.
        /// </summary>
        [ConfigurationProperty(EventLogNameProperty, IsRequired = false, DefaultValue = "")]
        public string EventLogName
        {
            get { return (string) this[EventLogNameProperty]; }
            set { this[EventLogNameProperty] = value; }
        }

        #endregion
    }
}
