using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Andamio.Data.Access
{
    /// <summary>
    /// Encapsulates the Settings for a Connection String.
    /// </summary>
    public sealed class DbConnectionSettings
    {
        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public DbConnectionSettings()
        {
        }

        /// <summary>
        /// Creates a new Instance while specifying the Connection String and Provider Name.
        /// </summary>
        /// <param name="connectionString">Connection String.</param>
        /// <param name="providerName">Provider Name.</param>
        public DbConnectionSettings(string connectionString, string providerName)
        {
            if (connectionString.IsNullOrBlank()) throw new ArgumentNullException("connectionString");
            if (providerName.IsNullOrBlank()) throw new ArgumentNullException("providerName");
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the Connection String.
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        /// Gets or Sets the Provider Name.
        /// </summary>
        public string ProviderName { get; set; }

        #endregion

        #region Load
        /// <summary>
        /// Creates an instance of DbConnectionStringSettings object based on specified Connection String.
        /// </summary>
        /// <param name="connectionStringName">The name of the connection string.</param>
        public static DbConnectionSettings FromName(string connectionStringName)
        {
            // Get Connection String and Provider type from config.
            ConnectionStringSettings connStringSettings = LoadConnectionString(connectionStringName);            
            return new DbConnectionSettings(connStringSettings.ConnectionString, connStringSettings.ProviderName);
        }

        /// <summary>
        /// Gets the System.Configuration.ConnectionStringsSection data for the specified Connection String Name.
        /// </summary>
        /// <param name="connectionStringName">The name of the connection string.</param>
        public static ConnectionStringSettings LoadConnectionString(string connectionStringName)
        {
            if (connectionStringName.IsNullOrBlank())
            { throw new ArgumentNullException("Invalid connection string name.", "connectionStringName"); }

            // gets the connection string from the config file
            ConnectionStringSettings connStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connStringSettings == null)
            {
                throw new ConfigurationErrorsException(String.Format("Could not find connection string settings for connection named '{0}'."
                    , connectionStringName));
            }

            return connStringSettings;
        }

        #endregion
    }
}
