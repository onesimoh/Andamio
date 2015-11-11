using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Andamio;
using Andamio.Threading;
using Andamio.Messaging.Formatters;

namespace Andamio.Messaging.Channels
{
    public abstract class BidirectionalChannel : ReceivingChannel, IChannelBroadcaster
    {
        #region Constructors
        protected BidirectionalChannel(IMessageSerializer serializer) : base(serializer)
        {
        }

        #endregion

        #region Broadcasting
        public abstract void Publish(RequestMessage request);
        public abstract void Publish(ReplyMessage reply);

        #endregion
    }
}
