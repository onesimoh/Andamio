using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

using Andamio;
using Andamio.Collections;
using Andamio.Messaging.Formatters;

namespace Andamio.Messaging.Channels
{
    public class ReceiverChannels : CollectionBase<IChannelReceiver>
    {
        #region Constructors
        public ReceiverChannels() : base()
        {
        }

        public ReceiverChannels(IEnumerable<ReceivingChannel> channels)
            : base(channels)
        {        
        }

        #endregion
    }


    public class BroadcastChannels : CollectionBase<IChannelBroadcaster>
    {
        #region Constructors
        public BroadcastChannels()
            : base()
        {
        }

        public BroadcastChannels(IEnumerable<BroadcastingChannel> channels)
            : base(channels)
        {
        }

        #endregion

        #region Broadcast
        public void Broadcast(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");
            this.ForEach(channel => channel.Publish(request));
        }

        public void Broadcast(ReplyMessage reply)
        {
            if (reply == null) throw new ArgumentNullException("request");
            this.ForEach(channel => channel.Publish(reply));
        }

        #endregion
    }


    public interface IChannel
    {
        IMessageSerializer Formatter { get; set; }
        
        event ItemEventHandler<Exception> ChannelError;
        void OnChannelError(ItemEventArgs<Exception> args);
        
        event EventHandler ChannelRecovery;
        void OnChannelRecovery(EventArgs args);
    }


    public static class ChannelFactory
    {
        public static IChannel Create(string type, string serializerType, NameValueCollection settings)
        {
            IMessageSerializer serializer = new GlobusOFSSerializer();
            IChannel channel = new FileSystemChannel(new FileSystemChannelSettings()
            {
                InboxPath = settings["inbox"],
                OutboxPath = settings["outbox"],
                ArchivePath = settings["archive"],
                ErrorPath = settings["error"],
                Extension = settings["extension"],
                Throttle = !settings["throttle"].IsNullOrBlank() ? TimeSpan.FromSeconds(Double.Parse(settings["throttle"])) : (TimeSpan?) null
            }, serializer);

            return channel;
        }
    }
}
