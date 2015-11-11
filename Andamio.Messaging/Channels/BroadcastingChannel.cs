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
    public interface IChannelBroadcaster : IChannel
    {
        void Publish(RequestMessage request);
        void Publish(ReplyMessage reply);
    }


    public abstract class BroadcastingChannel : IChannelBroadcaster
    {
        private BroadcastingChannel()
        {
        }

        protected BroadcastingChannel(IMessageSerializer formatter)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");
            Formatter = formatter;
        }

        public IMessageSerializer Formatter { get; set; }

        public abstract void Publish(RequestMessage request);
        public abstract void Publish(ReplyMessage reply);

        public event ItemEventHandler<Exception> ChannelError;
        public virtual void OnChannelError(ItemEventArgs<Exception> args)
        {
            if (ChannelError != null)
            {
                ChannelError.SafeEventInvoke(this, args);
            }
        }

        public void OnChannelError(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            OnChannelError(new ItemEventArgs<Exception>(exception));
        }

        public event EventHandler ChannelRecovery;
        public virtual void OnChannelRecovery(EventArgs args)
        {
            if (ChannelRecovery != null)
            {
                ChannelRecovery.SafeEventInvoke(this, args);
            }
        }

        public void OnChannelRecovery()
        {
            OnChannelRecovery(EventArgs.Empty);
        }
    }
}
