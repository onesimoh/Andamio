using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using Andamio.Configuration;

namespace Andamio.Diagnostics.Configuration
{
    /// <summary>
    /// Main Diagnosis Configuration Section.
    /// </summary>
    public sealed class DiagnosisConfigSection : ConfigurationSection
    {
        #region Private Fields, Constants
        /// <summary>
        /// The name of the Xml element that represents configuration section class. 
        /// </summary>
        public const string ElementName = "diagnosis";

        private const string XmlLogSettingsProperty = "xml";
        private const string SqliteSettingsProperty = "sqlite";
        private const string ConsoleLogSettingsProperty = "console";
        private const string DebugLogSettingsProperty = "debug";

        #endregion   
        
        #region Constructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public DiagnosisConfigSection() : base()
        {
        }

        /// <summary>
        /// Loads the Diagnosis Settings from Configuration.
        /// </summary>
        /// <returns>An instance of DiagnosisConfigSection</returns>
        public static DiagnosisConfigSection FromConfig()
        {
            return ConfigHelper.LoadConfiguration<DiagnosisConfigSection>(DiagnosisConfigSection.ElementName);
        }
        #endregion       
                
        #region Log Settings
        /// <summary>
        /// Gets or Sets the Xml Log Settings.
        /// </summary>
        [ConfigurationProperty(XmlLogSettingsProperty, IsRequired=false)]
        public XmlLogSettingsElement XmlLogSettings
        {
            get { return (XmlLogSettingsElement)this[XmlLogSettingsProperty]; }
            set { this[XmlLogSettingsProperty] = value; }
        }

        /// <summary>
        /// Gets or Sets the Database Log Settings.
        /// </summary>
        [ConfigurationProperty(SqliteSettingsProperty, IsRequired = false)]
        public SqliteSettingsElement SqliteSettings
        {
            get { return (SqliteSettingsElement)this[SqliteSettingsProperty]; }
            set { this[SqliteSettingsProperty] = value; }
        }

        /// <summary>
        /// Gets or Sets the Console Settings.
        /// </summary>
        [ConfigurationProperty(ConsoleLogSettingsProperty, IsRequired = false)]
        public ConsoleSettingsElement ConsoleLogSettings
        {
            get { return (ConsoleSettingsElement)this[ConsoleLogSettingsProperty]; }
            set { this[ConsoleLogSettingsProperty] = value; }
        }

        /// <summary>
        /// Gets or Sets the Debug Settings.
        /// </summary>
        [ConfigurationProperty(DebugLogSettingsProperty, IsRequired = false)]
        public DebugSettingsElement DebugLogSettings
        {
            get { return (DebugSettingsElement)this[DebugLogSettingsProperty]; }
            set { this[DebugLogSettingsProperty] = value; }
        }

        #endregion  
    }
}
