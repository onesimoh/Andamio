using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio.Security
{
    /// <summary>
    /// Represents a generic Security Exception.
    /// </summary>
    public class SecurityException : ApplicationException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SecurityException class with default properties. 
        /// </summary>
        public SecurityException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the SecurityException class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public SecurityException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the SecurityException class with a specified error message and inner exception that is the cause
        /// of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public SecurityException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        #endregion
    }
}
