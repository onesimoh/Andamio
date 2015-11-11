using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Security
{
    public class UserNotFoundException : NotFoundException
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        public UserNotFoundException() : base()
        {
        }

        /// <summary>
        /// Creates new instance using provided user.
        /// </summary>
        /// <param name="userName"></param>
        public UserNotFoundException(string userName) : base()
        {
            if (userName.IsNullOrBlank()) throw new ArgumentNullException("userName");
            UserName = userName;
        }

        /// <summary>
        /// Creates new instance using provided user and message.
        /// </summary>
        /// <param name="userName">User associated with exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UserNotFoundException(string userName, string message) : base(message)
        {
            if (userName.IsNullOrBlank()) throw new ArgumentNullException("userName");
            UserName = userName;
        }

        /// <summary>
        /// Gets user name associated with this exception.
        /// </summary>
        public string UserName { get; private set; }
    }
}
