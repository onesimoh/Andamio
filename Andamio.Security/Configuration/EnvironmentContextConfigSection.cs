using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using Andamio;
using Andamio.Configuration;

namespace Andamio.Security.Configuration
{
    public sealed class EnvironmentConfigSection : ConfigurationSection
    {
        #region Private Fields, Constants
        /// <summary>
        /// The name of the Xml element that represents configuration section class. 
        /// </summary>
        public const string ElementName = "environment";

        private const string EnvironmentContextTypeProperty = "context";
        #endregion   
        
        #region Constructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public EnvironmentConfigSection() : base()
        {
        }

        public static EnvironmentConfigSection FromConfig()
        {
            return ConfigHelper.LoadConfiguration<EnvironmentConfigSection>(EnvironmentConfigSection.ElementName);
        }

        #endregion                       

        #region Type
        [ConfigurationProperty(EnvironmentContextTypeProperty, IsRequired = true)]
        public EnvironmentContext Context
        {
            get { return this[EnvironmentContextTypeProperty].ToString().ParseEnum<EnvironmentContext>(); }
            set { this[EnvironmentContextTypeProperty] = value; }
        }

        #endregion
    }
}
