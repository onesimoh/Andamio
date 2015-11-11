using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio.Security
{
    /// <summary>
    /// Exception that is thrown when user cannot be authenticated.
    /// </summary>
    public class UserNotAuthenticatedException : SecurityException
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        public UserNotAuthenticatedException() : base()
        {

        }

        /// <summary>
        /// Creates new instance using provided user.
        /// </summary>
        /// <param name="userName">User associated with exception.</param>
        public UserNotAuthenticatedException(string userName) : base()
        {
            UserName = userName;
        }

        /// <summary>
        /// Creates new instance using provided user and message.
        /// </summary>
        /// <param name="userName">User associated with exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UserNotAuthenticatedException(string userName, string message) : base(message)
        {
            UserName = userName;
        }

        /// <summary>
        /// Gets user name associated with this exception.
        /// </summary>
        public string UserName { get; private set; }
    }
}
