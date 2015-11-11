using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Messaging
{
    public class ReplyException : ApplicationException
    {
        #region Constructors
        public ReplyException(string message, ReplyMessage reply)
            : base(message)
        {
            if (reply == null) throw new ArgumentNullException("reply");
            Reply = reply;
        }

        public ReplyException(string message, ReplyMessage reply, Exception innerException)
            : base(message, innerException)
        {
            if (reply == null) throw new ArgumentNullException("request");
            Reply = reply;
        }

        #endregion

        #region Reply
        public ReplyMessage Reply { get; private set; }

        #endregion
    }


    public class IgnoreReplyException : ReplyException
    {
        #region Constructors
        public IgnoreReplyException(string message, ReplyMessage reply)
            : base(message, reply)
        {
        }

        public IgnoreReplyException(string message, ReplyMessage reply, Exception innerException)
            : base(message, reply, innerException)
        {
        }

        #endregion
    }


    public class DuplicateReplyException : IgnoreReplyException
    {
        #region Constructors
        public DuplicateReplyException(string message, ReplyMessage reply)
            : base(message, reply)
        {
        }

        public DuplicateReplyException(string message, ReplyMessage reply, Exception innerException)
            : base(message, reply, innerException)
        {
        }

        #endregion
    }


    public class InvalidReplyException : IgnoreReplyException
    {
        #region Constructors
        public InvalidReplyException(string message, ReplyMessage reply)
            : base(message, reply)
        {
        }

        public InvalidReplyException(string message, ReplyMessage reply, Exception innerException)
            : base(message, reply, innerException)
        {
        }

        #endregion
    }
}
