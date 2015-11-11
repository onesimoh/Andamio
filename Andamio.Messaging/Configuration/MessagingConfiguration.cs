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
    public sealed class MessagingConfiguration : ConfigurationSection
    {
        #region Private Fields, Constants
        /// <summary>
        /// The name of the Xml element that represents configuration section class. 
        /// </summary>
        public const string ElementName = "messaging";

        private const string ChannelsProperty = "channels";

        #endregion   

        #region Constructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public MessagingConfiguration()
            : base()
        {
        }

        /// <summary>
        /// Loads the Messaging Settings from Configuration.
        /// </summary>
        public static MessagingConfiguration FromConfig()
        {
            return ConfigHelper.LoadConfiguration<MessagingConfiguration>(MessagingConfiguration.ElementName);
        }

        #endregion         
        
        #region Channels
        /// <summary></summary>
        [ConfigurationProperty(ChannelsProperty, IsRequired=true)]
        public ChannelsCollection Channels
        {
            get { return (ChannelsCollection) this[ChannelsProperty]; }
            set { this[ChannelsProperty] = value; }
        } 

        #endregion
    }
}
