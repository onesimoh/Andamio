using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Configuration;

namespace Andamio.Configuration
{
    /// <summary>
    /// Configuration Element that can be used to configured unknown attributes.
    /// </summary>
    public abstract class ParametersElement : ConfigurationElement
    {
        #region Constructor
        /// <summary>
        /// Default ParametersElement Constructor.
        /// </summary>
        protected ParametersElement()
        {
            Parameters = new NameValueCollection();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the unknown attributes used to configure this Element.
        /// </summary>
        public NameValueCollection Parameters { get; private set; }

        #endregion

        #region Overrides
        /// <summary>
        /// Gets the collection of properties.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection baseProperties = base.Properties;
                foreach (ConfigurationProperty dynprop in _dynamicProperties)
                {
                    baseProperties.Add(dynprop);
                }
                return baseProperties;
            }
        }

        private readonly ConfigurationPropertyCollection _dynamicProperties = new ConfigurationPropertyCollection();

        /// <summary>
        /// Gets a value indicating whether an unknown attribute is encountered during deserialization.
        /// </summary>
        /// <param name="name">The name of the unrecognized attribute.</param>
        /// <param name="value">The value of the unrecognized attribute.</param>
        /// <returns>true when an unknown attribute is encountered while deserializing.</returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            ConfigurationProperty dynamicProperty = new ConfigurationProperty(name, typeof(string), value);
            _dynamicProperties.Add(dynamicProperty);

            this[dynamicProperty] = value; // Add the value to values bag
            Parameters.Add(name, value);

            return true;
        }
        #endregion
    }
}
