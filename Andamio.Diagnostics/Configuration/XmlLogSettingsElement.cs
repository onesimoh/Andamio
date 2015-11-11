using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using Andamio.Configuration;
using Andamio.Diagnostics.Listeners;

namespace Andamio.Diagnostics.Configuration
{
    /// <summary>
    /// Xml Log Settings Diagnosis Configuration.
    /// </summary>
    public sealed class XmlLogSettingsElement : LogSettingsElementBase
    {
        #region Private Fields, Constants
        private const string MaxSizeInMBProperty = "maxSizeInMB";
        private const string FileLogModeProperty = "mode";
        private const string TraceFilePathProperty = "path";
        private const string TransformationFileProperty = "transformationFile";
        #endregion

        #region Constructor
        /// <summary>Default Constructor</summary>
        public XmlLogSettingsElement() : base()
        {
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or Sets the Max Size in MB for a Rolling File.
        /// </summary>
        [ConfigurationProperty(MaxSizeInMBProperty, IsRequired = false, DefaultValue = 0)]
        public int MaxSizeInMB
        {
            get { return (int)this[MaxSizeInMBProperty]; }
            set { this[MaxSizeInMBProperty] = value; }
        }

        /// <summary>
        /// Gets or Sets the File Log Mode.
        /// </summary>
        [ConfigurationProperty(FileLogModeProperty, IsRequired = false, DefaultValue = FileLogMode.Continuous)]
        public FileLogMode Mode
        {
            get { return (FileLogMode)this[FileLogModeProperty]; }
            set { this[FileLogModeProperty] = value; }
        }

        /// <summary>
        /// Gets or Sets the Log File Path.
        /// </summary>
        [ConfigurationProperty(TraceFilePathProperty, IsRequired=true)]
        public string FilePath
        {
            get { return (string)this[TraceFilePathProperty]; }
            set { this[TraceFilePathProperty] = value; }
        }

        /// <summary>
        /// Gets or Sets the optiona Xslt Transformation File to to parse the Xml Log into a more huma readable format.
        /// </summary>
        [ConfigurationProperty(TransformationFileProperty, IsRequired = false, DefaultValue = "")]
        public string TransformationFile
        {
            get { return (string) this[TransformationFileProperty]; }
            set { this[TransformationFileProperty] = value; }
        }

        #endregion
    }
}
