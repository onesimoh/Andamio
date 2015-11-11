using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio.Data.Exceptions
{
    /// <summary>
    /// Represents a generic Data Access Exception.
    /// </summary>
    /// <remarks>
    /// Users are encouraged to use this exception as a general mechanism to cascade up any exception generated from the Data Access Layer, 
    /// additional DataAccessException classes may be created by inheriting from this class. This class may optionally wrap any exception an a supplied message.
    /// </remarks>
    [Serializable]
    public class DataAccessException : ApplicationException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataAccessException()
            : base()
        {
        }

        /// <summary>
        /// Instantiates an new instance and supplies a message and inner exception.
        /// </summary>
        /// <param name="message">Message associated with generated exception.</param>
        /// <param name="e">Inner exception.</param>
        public DataAccessException(string message, Exception e)
            : base(message, e)
        {
        }

        /// <summary>
        /// Instantiates an new instance and supplies a message.
        /// </summary>
        /// <param name="message">Message associated with generated exception.</param>
        public DataAccessException(string message)
            : base(message)
        {
        }

        public DataAccessException(Exception e)
            : base(e.Message, e)
        {
        }
    }
}
