using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using Andamio;
using Andamio.Configuration;

namespace Andamio.Security.Configuration
{
    public sealed class SecurityAuthConfigSection : ConfigurationSection
    {
        #region Private Fields, Constants
        /// <summary>
        /// The name of the Xml element that represents configuration section class. 
        /// </summary>
        public const string ElementName = "security";

        private const string ActiveDirectoryPathProperty = "activeDirectoryPath";
        private const string ImpersonateProperty = "impersonate";
        #endregion   
        
        #region Constructor
        /// <summary>
        /// Default StorageConfigSection Constructor.
        /// </summary>
        public SecurityAuthConfigSection()
            : base()
        {
        }

        public static SecurityAuthConfigSection FromConfig()
        {
            SecurityAuthConfigSection securityConfig = null;
            ConfigHelper.TryLoadConfiguration<SecurityAuthConfigSection>(SecurityAuthConfigSection.ElementName, out securityConfig);
            return securityConfig;
        }
        #endregion       
                
        #region Connections
        [ConfigurationProperty(ActiveDirectoryPathProperty, IsRequired = true)]
        public string ActiveDirectoryPath
        {
            get { return (string)this[ActiveDirectoryPathProperty]; }
            set { this[ActiveDirectoryPathProperty] = value; }
        }

        #endregion  

        #region Impersonation
        [ConfigurationProperty(ImpersonateProperty, IsRequired = false)]
        public string Impersonate
        {
            get { return (string)this[ImpersonateProperty]; }
            set { this[ImpersonateProperty] = value; }
        }

        #endregion
    }
}
