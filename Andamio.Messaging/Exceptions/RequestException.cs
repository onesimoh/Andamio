using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Messaging
{
    public class RequestException : ApplicationException
    {
        #region Constructors
        public RequestException(string message, RequestMessage request)
            : base(message)
        {
            if (request == null) throw new ArgumentNullException("request");
            Request = request;
        }

        public RequestException(string message, RequestMessage request, Exception innerException)
            : base(message, innerException)
        {
            if (request == null) throw new ArgumentNullException("request");
            Request = request;
        }

        #endregion

        #region Request
        public RequestMessage Request { get; private set; }

        #endregion
    }


    public class IgnoreRequestException : RequestException
    {
        #region Constructors
        public IgnoreRequestException(string message, RequestMessage request)
            : base(message, request)
        {
        }

        public IgnoreRequestException(string message, RequestMessage request, Exception innerException)
            : base(message, request, innerException)
        {
        }

        #endregion
    }


    public class DuplicateRequestException : IgnoreRequestException
    {
        #region Constructors
        public DuplicateRequestException(string message, RequestMessage request)
            : base(message, request)
        {
        }

        public DuplicateRequestException(string message, RequestMessage request, Exception innerException)
            : base(message, request, innerException)
        {
        }

        #endregion
    }


    public class InvalidRequestException : IgnoreRequestException
    {
        #region Constructors
        public InvalidRequestException(string message, RequestMessage request)
            : base(message, request)
        {
        }

        public InvalidRequestException(string message, RequestMessage request, Exception innerException)
            : base(message, request, innerException)
        {
        }

        #endregion
    }
}
