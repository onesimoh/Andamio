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
    public class ReceivingChannelEventArgs<T> : EventArgs
        where T : Message
    {
        internal ReceivingChannelEventArgs(T message)
        {
            if (message == null) throw new ArgumentNullException("message");
            Message = message;
        }

        public T Message { get; private set; }
        public WorkItem AsynchronousHandle { get; internal set; }
    }

    public interface IChannelReceiver : IChannel
    {
        event EventHandler<ReceivingChannelEventArgs<RequestMessage>> RequestReceived;
        void OnRequestReceived(ReceivingChannelEventArgs<RequestMessage> args);

        event EventHandler<ReceivingChannelEventArgs<ReplyMessage>> ReplyReceived;
        void OnReplyReceived(ReceivingChannelEventArgs<ReplyMessage> args);

        void StartListening();
    }

    public abstract class ReceivingChannel : IChannelReceiver
    {
        private ReceivingChannel()
        {
        }

        protected ReceivingChannel(IMessageSerializer formatter)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");
            Formatter = formatter;
        }                

        public IMessageSerializer Formatter { get; set; }

        public event EventHandler<ReceivingChannelEventArgs<RequestMessage>> RequestReceived;
        public virtual void OnRequestReceived(ReceivingChannelEventArgs<RequestMessage> args)
        {
            if (RequestReceived != null)
            {
                RequestReceived.SafeEventInvoke(this, args);
            }            
        }

        public WorkItem OnRequestReceived(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");
            var args = new ReceivingChannelEventArgs<RequestMessage>(request);
            OnRequestReceived(args);
            return args.AsynchronousHandle;
        }

        public event EventHandler<ReceivingChannelEventArgs<ReplyMessage>> ReplyReceived;
        public virtual void OnReplyReceived(ReceivingChannelEventArgs<ReplyMessage> args)
        {
            if (ReplyReceived != null)
            {
                ReplyReceived.SafeEventInvoke(this, args);
            } 
        }

        public WorkItem OnReplyReceived(ReplyMessage reply)
        {
            if (reply == null) throw new ArgumentNullException("reply");
            var args = new ReceivingChannelEventArgs<ReplyMessage>(reply);
            OnReplyReceived(args);
            return args.AsynchronousHandle;
        }

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

        public abstract void StartListening();
    }
}
