using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Security
{
    public class NotFoundException : SecurityException                 
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        public NotFoundException() : base()
        {
        }

        /// <summary>
        /// Creates new instance using provided user and message.
        /// </summary>
        /// <param name="userName">User associated with exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public NotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SecurityException class with a specified error message and inner exception that is the cause
        /// of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
