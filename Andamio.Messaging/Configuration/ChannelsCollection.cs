using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Messaging.Configuration
{
    [ConfigurationCollection(typeof(ChannelElement), AddItemName = "channel")]
    public sealed class ChannelsCollection : ConfigurationElementCollection
    {
        #region Constructors
        public ChannelsCollection() : base()
        {
        }

        #endregion

        #region Overrides
        protected override ConfigurationElement CreateNewElement()
        {
            return new ChannelElement();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return base.CreateNewElement(elementName);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ChannelElement) element).Type;
        }

        #endregion
    }
}
