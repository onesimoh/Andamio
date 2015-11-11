using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Andamio.Configuration;
using Andamio.Diagnostics.Listeners;

namespace Andamio.Diagnostics.Configuration
{
    /// <summary>
    /// Sqlite Settings Diagnosis Configuration.
    /// </summary>
    public sealed class SqliteSettingsElement : LogSettingsElementBase
    {
        #region Private Fields, Constants
        private const string PathProperty = "path";

        #endregion

        #region Constructor
        /// <summary>Default Constructor</summary>
        public SqliteSettingsElement()
            : base()
        {
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or Sets the Log File Path.
        /// </summary>
        [ConfigurationProperty(PathProperty, IsRequired = true)]
        public string FilePath
        {
            get { return (string)this[PathProperty]; }
            set { this[PathProperty] = value; }
        }

        #endregion
    }
}
