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
using Andamio.Cryptography;
using Andamio.Data;
using Andamio.Data.Access;
using Andamio.Data.Entities;
using Andamio.Data.Transactions;

namespace Andamio.Messaging
{
    #region Direction
    public enum MessageDirection
    {
        Unknown = 0,
        Incoming = 1,
        Outgoing = 2,
    }

    public static class MessageDirectionExtensions
    {
        public static bool IsDefined(this MessageDirection direction)
        {
            return direction != MessageDirection.Unknown;
        }
        public static bool IsIncoming(this MessageDirection direction)
        {
            return direction == MessageDirection.Incoming;
        }
        public static bool IsOutgoing(this MessageDirection direction)
        {
            return direction == MessageDirection.Outgoing;
        }
        public static MessageDirection Revert(this MessageDirection direction)
        {
            if (!direction.IsDefined()) throw new InvalidOperationException();
            return direction.IsIncoming() ? MessageDirection.Outgoing : MessageDirection.Incoming;
        }

    }

    #endregion


    #region Status
    public enum MessageStatus
    {
        None = 0,
        Success = 10,
        Failure = 20,
        Invalid = 30,
    }

    public static class MessageStatusExtensions
    {
        public static bool IsDefined(this MessageStatus status)
        {
            return status != MessageStatus.None;
        }
        public static bool IsSuccess(this MessageStatus status)
        {
            return status == MessageStatus.Success;
        }
        public static bool IsFailure(this MessageStatus status)
        {
            return status == MessageStatus.Failure;
        }
        public static bool IsInvalid(this MessageStatus status)
        {
            return status == MessageStatus.Invalid;
        }
    }

    #endregion


    [DebuggerDisplay("{Event}: {CorrelationId}")]
    public abstract class Message
    {
        #region Constructors
        protected Message()
        {
            CorrelationId = Guid.NewGuid().ToString();
            TimeStamp = DateTime.Now;
            Owner = User.GetCurrentFromAD();
        }

        public Message(string correlationId, string eventName)
            : this()
        {
            if (correlationId.IsNullOrBlank()) throw new ArgumentNullException("correlationId");
            if (eventName.IsNullOrBlank()) throw new ArgumentNullException("eventName");
            Event = eventName;
            CorrelationId = correlationId;
        }

        #endregion

        #region Conversion
        public static implicit operator MessageEvent(Message message)
        {
            if (message == null) throw new ArgumentNullException("message");

            MessageEventKind kind;
            if (message is RequestMessage)
            { kind = MessageEventKind.Request; }
            else if (message is ReplyMessage)
            { kind = MessageEventKind.Reply; }
            else
            { kind = MessageEventKind.Undefined; }

            return new MessageEvent()
            {
                CorrelationId = message.CorrelationId,
                Direction = (MessageEventDirection) message.Direction,
                Kind = kind,
                Event = message.Event,
                Status = MessageEventStatus.Unknown,
                Environment = message.Environment,
                Application = message.Application,
                Version = message.Version,
                Owner = (message.Owner != null) ? message.Owner.UserName : null,
                TimeStamp = message.TimeStamp,
                Content = message.Content,
                Original = message.Original,
                Method = message.Method,
                State = message.State,
                Thumbprint = message.Thumbprint(),
            };
        }

        #endregion

        #region Properties
        public string CorrelationId { get; set; }
        public MessageStatus Status { get; internal set; }
        public MessageDirection Direction { get; internal set; }
        public string Event { get; protected set; }
        public string Environment { get; set; }
        public string Application { get; set; }
        public string Version { get; set; }
        public string Method { get; set; }
        public string State { get; set; }
        public User Owner { get; set; }
        public DateTime TimeStamp { get; protected set; }
        public string Original { get; internal set; }
        
        #endregion

        #region Content
        private DynamicContent _content = new DynamicContent();
        public dynamic Content 
        {
            get { return _content; }
            set { _content = DynamicContent.From(value); }
        }

        public virtual string Thumbprint()
        {
            return CryptoUtils.GetMD5Hash(Original).ToHexString();
        }

        #endregion

        #region Serialization
        /*
        public virtual dynamic Write()
        {
            return new
            {
                CorrelationId,
                Status,
                Direction,
                Event,
                Environment,
                Application,
                Version,
                Owner = (Owner != null) ? Owner.UserName : null,
                TimeStamp,
                Content,
            };
        }

        public virtual void Read(dynamic value)
        {
            throw new NotImplementedException();
        }
        */

        #endregion
    }
}
