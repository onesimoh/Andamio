using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio.Security
{
    /// <summary>
    /// CriticalSecurityException is thrown when a critical security exception occurs that may require the application to shutdown.
    /// </summary>
    public class CriticalSecurityException : SecurityException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CriticalSecurityException class with default properties. 
        /// </summary>
        public CriticalSecurityException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CriticalSecurityException class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>          
        public CriticalSecurityException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CriticalSecurityException class with a specified error message and inner exception that is the cause
        /// of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public CriticalSecurityException(string message, Exception innerException)
            : base(message, innerException)
        {            
        }

        #endregion
    }
}
