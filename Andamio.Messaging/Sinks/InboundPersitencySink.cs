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
    public class InboundPersitencySink : IMessageSink
    {
        void IMessageSink.Invoke(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");

            MessageEvent incomingRequest = DAO.For<MessageEvent>().Single(match => match.CorrelationId == request.CorrelationId
                && match.Kind == MessageEventKind.Request && match.Direction == MessageEventDirection.Incoming
                && match.Thumbprint == request.Thumbprint());
            if (incomingRequest != null)
            {
                throw new DuplicateRequestException(String.Format("Incoming Request '{0}' for Event '{1}' was already processed. Request will be Ignored!"
                    , request.CorrelationId
                    , request.Event)
                    , request);
            }

            DAO.For<MessageEvent>().Upsert(request);
        }

        void IMessageSink.Invoke(ReplyMessage reply)
        {
            if (reply == null) throw new ArgumentNullException("reply");

            MessageEvent outgoingRequest = DAO.For<MessageEvent>().Single(match => match.CorrelationId == reply.CorrelationId
                && match.Kind == MessageEventKind.Request && match.Direction == MessageEventDirection.Outgoing);
            if (outgoingRequest == null)
            {
                throw new InvalidReplyException(String.Format("A corresponding Outgoing Request was never Sent for Incoming Reply '{0}' and Event '{1}'. Reply will be Ignored!"
                    , reply.CorrelationId
                    , reply.Event)
                    , reply);
            }

            MessageEvent incomingReply = DAO.For<MessageEvent>().Single(match => match.CorrelationId == reply.CorrelationId
                && match.Kind == MessageEventKind.Reply && match.Direction == MessageEventDirection.Incoming
                && match.Thumbprint == reply.Thumbprint());
            if (incomingReply != null)
            {
                throw new DuplicateReplyException(String.Format("Incoming Reply '{0}' for Event '{1}' was already processed. Reply will be Ignored!"
                    , reply.CorrelationId
                    , reply.Event)
                    , reply);
            }

            DAO.For<MessageEvent>().Upsert(reply);
        }
    }
}
