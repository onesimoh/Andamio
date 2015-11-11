using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

using Andamio;
using Andamio.Threading;
using Andamio.Serialization;

namespace Andamio.Messaging
{
    [DebuggerDisplay("{Event}: {CorrelationId}")]
    public class ReplyMessage : Message
    {
        #region Constructors
        private ReplyMessage() : base()
        {
        }

        public ReplyMessage(string correlationId, string eventName)
            : base(correlationId, eventName)
        {
        }

        public static ReplyMessage Success(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");
            return new ReplyMessage()
            {
                CorrelationId = request.CorrelationId,
                Direction = request.Direction.Revert(),
                Event = request.Event,
                Status = MessageStatus.Success,
                Environment = request.Environment,
                Application = request.Application,
                Version = request.Version,
                Owner = request.Owner,
                TimeStamp = DateTime.UtcNow,
            };
        }

        public static ReplyMessage Failure(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");
            return new ReplyMessage()
            {
                CorrelationId = request.CorrelationId,
                Direction = request.Direction.Revert(),
                Event = request.Event,
                Status = MessageStatus.Failure,
                Environment = request.Environment,
                Application = request.Application,
                Version = request.Version,
                Owner = request.Owner,
                TimeStamp = DateTime.UtcNow,
            };
        }

        #endregion
    }
}
