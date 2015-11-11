using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Andamio;
using Andamio.Collections;
using Andamio.Diagnostics.Listeners;
using Andamio.Diagnostics.Configuration;
using Andamio.Security;

namespace Andamio.Diagnostics
{
    #region TraceListenerSettings
    /// <summary>
    /// Defines the settings for the Xml Trace Listener.
    /// </summary>
    public abstract class TraceListenerSettings
    {
        /// <summary>Default Constructor</summary>
        public TraceListenerSettings()
        {
        }

        /// <summary>
        /// Gets or Sets the customr listener.
        /// </summary>
        public string Listener { get; set; }

        /// <summary>
        /// Gets or Sets the Event Log Name.
        /// </summary>
        public string EventLogName { get; set; }

        /// <summary>
        /// Creates the listener from the current Settings.
        /// </summary>
        /// <returns>A Log Recorder Listener instance.</returns>
        public virtual ILogRecorder CreateListener()
        {
            if (!Listener.IsNullOrBlank())
            {
                var args = new TraceListenerSettings[] { this };
                return Activator.CreateInstance(Type.GetType(Listener), args) as ILogRecorder;
            }

            return null;
        }
    }

    #endregion
}
