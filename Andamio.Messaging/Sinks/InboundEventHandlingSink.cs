using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Andamio;

namespace Andamio.Messaging.Sinks
{
    public class InboundEventHandlingSink : IMessageSink
    {
        void IMessageSink.Invoke(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");

            DistributedMessaging messaging = DistributedMessaging.Instance;
            Action<RequestMessage> eventHandler = messaging.Handlers.Incoming.Request(request.Event);
            if (eventHandler != null)
            {
                eventHandler.DynamicInvoke(request);
            }
        }

        void IMessageSink.Invoke(ReplyMessage reply)
        {
            if (reply == null) throw new ArgumentNullException("reply");

            DistributedMessaging messaging = DistributedMessaging.Instance;
            Action<ReplyMessage> eventHandler = messaging.Handlers.Incoming.Reply(reply.Event);
            if (eventHandler != null)
            {
                eventHandler.DynamicInvoke(reply);
            }
        }
    }
}
