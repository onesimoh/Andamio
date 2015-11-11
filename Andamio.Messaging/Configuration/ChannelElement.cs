using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Andamio;
using Andamio.Configuration;

namespace Andamio.Messaging.Configuration
{
    #region Settings
    public sealed class ChannelElementSettings : ParametersElement
    {
    }

    #endregion


    public sealed class ChannelElement : ConfigurationElement
    {
        #region Private Fields, Constants
        /// <summary>
        /// The name of the Xml element that represents configuration section class. 
        /// </summary>
        public const string ElementName = "channel";

        private const string TypeProperty = "type";
        private const string DirectionProperty = "direction";
        private const string SerializerProperty = "serializer";
        private const string SettingsProperty = "settings";


        #endregion   
        
        #region Constructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ChannelElement() : base()
        {
        }

        #endregion

        #region Properties
        /// <summary>
        /// Type.
        /// </summary>
        [ConfigurationProperty(TypeProperty, IsRequired = true)]
        public string Type
        {
            get { return (string) this[TypeProperty]; }
            set { this[TypeProperty] = value; }
        }

        /// <summary>
        /// Direction.
        /// </summary>
        [ConfigurationProperty(DirectionProperty, IsRequired = true)]
        public MessageEventDirection Direction
        {
            get { return (MessageEventDirection) this[DirectionProperty]; }
            set { this[DirectionProperty] = value; }
        }

        /// <summary>
        /// Serializer.
        /// </summary>
        [ConfigurationProperty(SerializerProperty, IsRequired = true)]
        public string Serializer
        {
            get { return (string) this[SerializerProperty]; }
            set { this[SerializerProperty] = value; }
        }

        /// <summary>
        /// Settings.
        /// </summary>
        [ConfigurationProperty(SettingsProperty, IsRequired = false)]
        public ChannelElementSettings Settings
        {
            get { return (ChannelElementSettings)this[SettingsProperty]; }
            set { this[SettingsProperty] = value; }
        }

        #endregion
    }
}
