using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Andamio;
using Andamio.Serialization;
using Andamio.Data.Access;

namespace Andamio.Messaging
{
    [DebuggerDisplay("{Event}: {CorrelationId}")]
    public class RequestMessage : Message
    {
        #region Constructors
        private RequestMessage() : base()
        {
        }

        public RequestMessage(string correlationId, string eventName)
            : base(correlationId, eventName)
        {
        }

        #endregion

        #region Factory
        public static RequestMessage Create(string correlationId, string method = null)
        {
            MessageEvent message = DAO.For<MessageEvent>().Single(match => match.CorrelationId == correlationId
                && match.Direction == MessageEventDirection.Incoming && match.Kind == MessageEventKind.Request);

            if (message == null)
            {
                throw new InvalidOperationException(String.Format("A corresponding Incoming Request Message for '{0}' was not found!", correlationId));
            }

            return new RequestMessage(correlationId, message.Event)
            {
                Application = message.Application,
                Version = message.Version,
                Environment = message.Environment,
                Method = method,
                Owner = User.CurrentFromDB()
            };
        }
        #endregion
    }
}
