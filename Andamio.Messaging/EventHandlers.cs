using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Andamio;

namespace Andamio.Messaging
{
    public interface IRequestMessageHandler
    {
        void Invoke(RequestMessage request);
    }

    public interface IReplyMessageHandler
    {
        void Invoke(ReplyMessage reply);
    }

    public class EventHandlers
    {
        #region Constructors
        internal EventHandlers()
        {
        }

        #endregion

        #region Request
        private Dictionary<String, Action<RequestMessage>> _requestEventHandlers = new Dictionary<String, Action<RequestMessage>>();
        public Action<RequestMessage> Request(string eventName)
        {
            if (eventName.IsNullOrBlank()) throw new ArgumentNullException("eventName");
            Action<RequestMessage> events;
            if (_requestEventHandlers.TryGetValue(eventName, out events))
            { return events; }
            return null;
        }

        public void Request(string eventName, Action<RequestMessage> method)
        {
            if (eventName.IsNullOrBlank()) throw new ArgumentNullException("eventName");
            if (method == null) throw new ArgumentNullException("method");
            if (_requestEventHandlers.ContainsKey(eventName))
            {
                _requestEventHandlers[eventName] += method;
            }
            else
            {
                _requestEventHandlers[eventName] = method;
            }
        }

        public void Request(string eventName, IRequestMessageHandler eventHandler)
        {
            if (eventHandler == null) throw new ArgumentNullException("eventHandler");
            Request(eventName, new Action<RequestMessage>(eventHandler.Invoke));
        }
        
        #endregion

        #region Reply
        private Dictionary<String, Action<ReplyMessage>> _replyEventHandlers = new Dictionary<String, Action<ReplyMessage>>();
        public Action<ReplyMessage> Reply(string eventName)
        {
            if (eventName.IsNullOrBlank()) throw new ArgumentNullException("eventName");
            Action<ReplyMessage> events;
            if (_replyEventHandlers.TryGetValue(eventName, out events))
            { return events; }
            return null;
        }

        public void Reply(string eventName, Action<ReplyMessage> method)
        {
            if (eventName.IsNullOrBlank()) throw new ArgumentNullException("eventName");
            if (method == null) throw new ArgumentNullException("method");
            if (_replyEventHandlers.ContainsKey(eventName))
            {
                _replyEventHandlers[eventName] += method;
            }
            else
            {
                _replyEventHandlers[eventName] = method;
            }
        }

        public void Reply(string eventName, IReplyMessageHandler eventHandler)
        {
            if (eventHandler == null) throw new ArgumentNullException("eventHandler");
            Reply(eventName, new Action<ReplyMessage>(eventHandler.Invoke));
        }

        #endregion
    }
}
