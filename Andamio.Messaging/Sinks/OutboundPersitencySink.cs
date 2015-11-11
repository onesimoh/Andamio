using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Andamio;
using Andamio.Data;
using Andamio.Data.Access;

namespace Andamio.Messaging.Sinks
{
    public class OutboundPersitencySink : IMessageSink
    {
        void IMessageSink.Invoke(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");

            MessageEvent outgoingRequest = DAO.For<MessageEvent>().Single(match => match.CorrelationId == request.CorrelationId
                && match.Kind == MessageEventKind.Request && match.Direction == MessageEventDirection.Outgoing
                && match.Thumbprint == request.Thumbprint());
            if (outgoingRequest != null)
            {
                throw new DuplicateRequestException(String.Format("Outgoing Request '{0}' for Event '{1}' was already processed. Request will be Ignored!"
                    , request.CorrelationId
                    , request.Event)
                    , request);
            }

            MessageEvent messageEvent = request;
            messageEvent.Original = DynamicContent.From(request);
            DAO.For<MessageEvent>().Upsert(messageEvent);
        }

        void IMessageSink.Invoke(ReplyMessage reply)
        {
            if (reply == null) throw new ArgumentNullException("reply");

            MessageEvent incomingRequest = DAO.For<MessageEvent>().Single(match => match.CorrelationId == reply.CorrelationId
                && match.Kind == MessageEventKind.Request && match.Direction == MessageEventDirection.Incoming);
            if (incomingRequest == null)
            {
                throw new InvalidReplyException(String.Format("A corresponding Incoming Request was never Received for Outgoing Reply '{0}' and Event '{1}'. Reply will be Ignored!"
                    , reply.CorrelationId
                    , reply.Event)
                    , reply);
            }

            MessageEvent outgoingReply = DAO.For<MessageEvent>().Single(match => match.CorrelationId == reply.CorrelationId
                && match.Kind == MessageEventKind.Reply && match.Direction == MessageEventDirection.Outgoing
                && match.Thumbprint == reply.Thumbprint());
            if (outgoingReply != null)
            {
                throw new DuplicateReplyException(String.Format("Outgoing Reply '{0}' for Event '{1}' was already processed. Reply will be Ignored!"
                    , reply.CorrelationId
                    , reply.Event)
                    , reply);
            }

            MessageEvent messageEvent = reply;
            messageEvent.Original = DynamicContent.From(reply);
            DAO.For<MessageEvent>().Upsert(messageEvent);
        }
    }
}
