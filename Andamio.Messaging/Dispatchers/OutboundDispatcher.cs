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
using Andamio.Collections;
using Andamio.Threading;
using Andamio.Diagnostics;
using Andamio.Messaging.Sinks;
using Andamio.Messaging.Channels;

namespace Andamio.Messaging.Dispatchers
{
    public class OutboundDispatcher : Disposable
    {
        #region Constructors
        internal OutboundDispatcher()
        {
            Queue.Start();

            Channels = new BroadcastChannels();
            Channels.ItemsInserted += OnChannelsInserted;

            Sinks = new SinkPipeline();
            Sinks.Add(new OutboundPersitencySink(), new OutboundEventHandlingSink());
        }

        #endregion

        #region Channels
        public BroadcastChannels Channels { get; private set; }
        void OnChannelsInserted(object sender, ItemEventArgs<IEnumerable<IChannelBroadcaster>> e)
        {
            var channels = e.Item;
            if (channels != null)
            {
                foreach (IChannelBroadcaster channel in channels)
                {
                    channel.ChannelError += OnChannelError;
                }
            }
        }

        void OnChannelError(object sender, ItemEventArgs<Exception> e)
        {
            Exception exception = e.Item;
            Logger.Error(String.Format("An unexpected Broadcast Channel Error occurred. Channel: '{0}'."
                , sender.GetType())
                , exception);
        }

        #endregion

        #region Sinks
        public SinkPipeline Sinks { get; private set; }

        #endregion

        #region Queuing
        readonly WorkerThreadQueue Queue = new WorkerThreadQueue();

        public WorkItem Push(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");

            request.Direction = MessageDirection.Outgoing;

            Logger.Trace(String.Format("Outgoing Request '{0}' for Event '{1}' has been queued."
                , request.CorrelationId
                , request.Event)
                /*, new XmlLogAttachment("Request", request)*/);

            Action sinkChainInvocation = () => Sinks.InvokeTransactional(request);
            WorkItem workItem = new WorkItem(request, sinkChainInvocation);
            
            workItem.Completed += delegate(object sender, EventArgs e)
            {
                Logger.Trace(String.Format("Outgoing Request '{0}' for Event '{1}' was successfully processed."
                    , request.CorrelationId
                    , request.Event)
                    /*, new XmlLogAttachment("Request", request)*/);

                Channels.Broadcast(request);
            };

            workItem.Error += delegate(object sender, ItemEventArgs<Exception> e)
            {
                Exception exception = e.Item;
                Logger.Error(String.Format("Error occurred while processing Outgoing Request '{0}' for Event '{1}'."
                    , request.CorrelationId
                    , request.Event)
                    , exception
                    /*, new XmlLogAttachment("Request", request)*/);
            };

            Queue.QueueWorkItem(workItem);
            return workItem;
        }

        public WorkItem Push(ReplyMessage reply)
        {
            if (reply == null) throw new ArgumentNullException("reply");

            reply.Direction = MessageDirection.Outgoing;

            Logger.Trace(String.Format("Outgoing Reply '{0}' for Event '{1}' has been queued."
                , reply.CorrelationId
                , reply.Event)
                /*, new XmlLogAttachment("Request", request)*/);

            Action sinkChainInvocation = () => Sinks.InvokeTransactional(reply);
            WorkItem workItem = new WorkItem(reply, sinkChainInvocation);

            workItem.Completed += delegate(object sender, EventArgs e)
            {
                Logger.Trace(String.Format("Outgoing Reply '{0}' for Event '{1}' was successfully processed."
                    , reply.CorrelationId
                    , reply.Event)
                    /*, new XmlLogAttachment("Reply", reply)*/);

                Channels.Broadcast(reply);
            };

            workItem.Error += delegate(object sender, ItemEventArgs<Exception> e)
            {
                Exception exception = e.Item;
                Logger.Error(String.Format("Error occurred while processing Outgoing Request '{0}' for Event '{1}'."
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
