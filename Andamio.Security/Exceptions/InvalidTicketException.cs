using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Security
{
    public class InvalidTicketException : CriticalSecurityException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the InvalidAuthTicketException class with default properties. 
        /// </summary>
        public InvalidTicketException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidAuthTicketException class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidTicketException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidAuthTicketException class with a specified error message and inner exception that is the cause
        /// of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InvalidTicketException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
