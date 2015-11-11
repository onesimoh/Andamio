using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Configuration;
using System.Reflection;

namespace Andamio.Configuration
{
    /// <summary>
    /// Utility class to help in loading/retreiving configuration sections and values.
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Loads a configuration section.
        /// </summary>
        /// <typeparam name="T">Generic type of the configuration section to lead.</typeparam>
        /// <param name="sectionName">Name of configuration section to load.</param>
        /// <returns>Returns Configuration Section.</returns>
        public static T LoadConfiguration<T>(string sectionName) where T : ConfigurationSection
        {
            T section = ConfigurationManager.GetSection(sectionName) as T;
            if (section == null)
            {
                throw new ConfigurationErrorsException(String.Format("Configuration section missing. Name = {0}, Type = {1}.", sectionName, typeof(T)));
            }
            return section;
        }

        public static bool TryLoadConfiguration<T>(string sectionName, out T configSection) where T : ConfigurationSection
        {
            configSection = ConfigurationManager.GetSection(sectionName) as T;
            return configSection != null;
        }

        /// <summary>
        /// Gets the connection string with specified name.
        /// </summary>
        /// <param name="connectionName">The name of the connection string to retrieve.</param>
        /// <returns>Returns the connection string with specified name.</returns>
        public static ConnectionStringSettings GetConnectionString(string connectionStringName)
        {
            //get the connection string from the config file
            ConnectionStringSettings connStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connStringSettings == null)
            {
                throw new ConfigurationErrorsException(String.Format("Could not find connection string settings for connection named '{0}'."
                    , connectionStringName));
            }
            return connStringSettings;
        }

        #region Private Methods
        private static bool TryParse(string s, Type type, out object result)
        {
            result = null;

            if (String.IsNullOrEmpty(s))
            { return false; }

            try
            {
                result = Convert.ChangeType(s, type);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}