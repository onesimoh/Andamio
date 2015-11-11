using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Diagnostics;

using Andamio;
using Andamio.Data;
using Andamio.Data.Transactions;
using Andamio.Collections;
using Andamio.Threading;
using Andamio.Diagnostics;
using Andamio.Messaging.Sinks;
using Andamio.Messaging.Channels;

namespace Andamio.Messaging.Dispatchers
{
    public class InboundDispatcher : Disposable
    {
        #region Constructors
        internal InboundDispatcher()
        {
            Queue.Start();

            Channels = new ReceiverChannels();
            Channels.ItemsInserted += OnChannelsInserted;

            Sinks = new SinkPipeline();
            Sinks.Add(new InboundPersitencySink(), new InboundEventHandlingSink());
        }

        #endregion

        #region Channels
        public ReceiverChannels Channels { get; private set; }
        void OnChannelsInserted(object sender, ItemEventArgs<IEnumerable<IChannelReceiver>> e)
        {
            var channels = e.Item;
            if (channels != null)
            {
                foreach (ReceivingChannel channel in channels)
                {
                    channel.RequestReceived += OnRequestReceived;
                    channel.ReplyReceived += OnReplyReceived;                    
                    channel.ChannelError += OnChannelError;
                    channel.ChannelRecovery += OnChannelRecovery;
                    channel.StartListening();
                }
            }
        }

        void OnRequestReceived(object sender, ReceivingChannelEventArgs<RequestMessage> e)
        {
            if (e.Message != null)
            {
                e.AsynchronousHandle = Pull(e.Message);                 
            }
        }

        void OnReplyReceived(object sender, ReceivingChannelEventArgs<ReplyMessage> e)
        {
            if (e.Message != null)
            {
                e.AsynchronousHandle = Pull(e.Message);                
            }
        }

        void OnChannelError(object sender, ItemEventArgs<Exception> e)
        {
            Exception exception = e.Item;
            Logger.Error(String.Format("An unexpected Receiver Channel Error occurred. Channel: '{0}'."
                , sender.GetType())
                , exception);
        }

        void OnChannelRecovery(object sender, EventArgs e)
        {
            Logger.Info(String.Format("Receiver Channel recovered from an unexpected Fault. Channel: '{0}'.", sender.GetType()));            
        }

        #endregion

        #region Sinks
        public SinkPipeline Sinks { get; private set; }

        #endregion

        #region Queuing
        readonly WorkerThreadQueue Queue = new WorkerThreadQueue(throttle: 1);

        public WorkItem Pull(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");

            request.Direction = MessageDirection.Incoming;

            Logger.Trace(String.Format("Incoming Request '{0}' for Event '{1}' has been queued."
                , request.CorrelationId
                , request.Event)
                /*, new XmlLogAttachment("Request", request)*/);

            Action sinkChainInvocation = () => Sinks.InvokeTransactional(request);
            WorkItem workItem = new WorkItem(request, sinkChainInvocation);
            
            workItem.Completed += delegate(object sender, EventArgs e)
            {
                Logger.Trace(String.Format("Incoming Request '{0}' for Event '{1}' was successfully processed."
                    , request.CorrelationId
                    , request.Event)
                    /*, new XmlLogAttachment("Request", request)*/);

                ReplyMessage reply = ReplyMessage.Success(request);
                //DistributedMessaging.Instance.Dispatchers.Outgoing.Push(reply);
            };
            
            workItem.Error += delegate(object sender, ItemEventArgs<Exception> e)
            {
                Exception exception = e.Item;
                Logger.Error(String.Format("Error occurred while processing Incoming Request '{0}' for Event '{1}'."
                    , request.CorrelationId
                    , request.Event)
                    , exception
                    /*, new XmlLogAttachment("Request", request)*/);

                if (exception is IgnoreRequestException)
                { return; }

                ReplyMessage reply = ReplyMessage.Failure(request);
                //DistributedMessaging.Instance.Dispatchers.Outgoing.Push(reply);
            };

            Queue.QueueWorkItem(workItem);
            return workItem;
        }

        public WorkItem Pull(ReplyMessage reply)
        {
            if (reply == null) throw new ArgumentNullException("reply");

            reply.Direction = MessageDirection.Incoming;

            Logger.Trace(String.Format("Incoming Reply '{0}' for Event '{1}' has been queued."
                , reply.CorrelationId
                , reply.Event)
                /*, new XmlLogAttachment("Request", request)*/);

            Action sinkChainInvocation = () => Sinks.InvokeTransactional(reply);
            WorkItem workItem = new WorkItem(reply, sinkChainInvocation);
            
            workItem.Completed += delegate(object sender, EventArgs e)
            {
                Logger.Trace(String.Format("Incoming Reply '{0}' for Event '{1}' was successfully processed."
                    , reply.CorrelationId
                    , reply.Event)
                    /*, new XmlLogAttachment("Reply", reply)*/);
            };

            workItem.Error += delegate(object sender, ItemEventArgs<Exception> e)
            {
                Exception exception = e.Item;
                Logger.Error(String.Format("Error occurred while processing Incoming Reply '{0}' for Event '{1}'."
                    , reply.CorrelationId
                    , reply.Event)
                    , exception
                    /*, new XmlLogAttachment("Reply", reply)*/);
            };

            Queue.QueueWorkItem(workItem);
            return workItem;
        }

        #endregion

        #region Disposable
        protected override void Cleanup()
        {
            Queue.Stop();
            Queue.Dispose();
        }

        #endregion
    }
}
