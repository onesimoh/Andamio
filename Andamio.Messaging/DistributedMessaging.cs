using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Andamio;
using Andamio.Messaging;
using Andamio.Messaging.Channels;
using Andamio.Messaging.Dispatchers;
using Andamio.Messaging.Configuration;

namespace Andamio.Messaging
{
    public sealed class DispatchersConfiguration
    {
        #region Constructors
        internal DispatchersConfiguration()
        {
            Incoming = new InboundDispatcher();
            Outgoing = new OutboundDispatcher();
        }

        #endregion

        #region Dispatchers
        public InboundDispatcher Incoming { get; private set; }
        public OutboundDispatcher Outgoing { get; private set; }

        #endregion
    }


    public sealed class EventHandlersConfiguration
    {
        #region Constructors
        internal EventHandlersConfiguration()
        {
            Incoming = new EventHandlers();            
            Outgoing = new EventHandlers();                    
        }

        #endregion

        #region Handlers
        public EventHandlers Incoming { get; private set; }
        public EventHandlers Outgoing { get; private set; }

        #endregion
    }


    public sealed class DistributedMessaging
    {
        #region Constructors
        public readonly static DistributedMessaging Instance = new DistributedMessaging();
        private DistributedMessaging()
        {
            Dispatchers = new DispatchersConfiguration();
            Handlers = new EventHandlersConfiguration();
        }

        public static DistributedMessaging InitializeFromConfig()
        {
            MessagingConfiguration config = MessagingConfiguration.FromConfig();
            foreach (ChannelElement channelElement in config.Channels)
            {
                IChannel channel = ChannelFactory.Create(channelElement.Type, channelElement.Serializer, channelElement.Settings.Parameters);
                switch (channelElement.Direction)
                {
                    case MessageEventDirection.Incoming:
                        Instance.Dispatchers.Incoming.Channels.Add((IChannelReceiver) channel);
                        break;
                    case MessageEventDirection.Outgoing:
                        Instance.Dispatchers.Outgoing.Channels.Add((IChannelBroadcaster) channel);
                    break;
                    case MessageEventDirection.Bidirectional:
                        Instance.Dispatchers.Incoming.Channels.Add((IChannelReceiver) channel);
                        Instance.Dispatchers.Outgoing.Channels.Add((IChannelBroadcaster) channel);
                    break;
                    default:
                    break;
                }
            }

            return Instance;
        }

        #endregion

        #region Dispatchers
        public DispatchersConfiguration Dispatchers { get; private set; }

        #endregion

        #region Handlers
        public EventHandlersConfiguration Handlers { get; private set; }

        #endregion

        #region Delivery
        public void Send(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");
            Dispatchers.Outgoing.Push(request);
        }

        #endregion



    }
}
