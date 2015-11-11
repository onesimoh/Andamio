using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Andamio.Data
{
    [Serializable]
    public class CheckedInFileException : ApplicationException
    {
        private const string ErrorMessage = "Access to Read-Only file '{0}' was denied.\n\nThis is probably becaused the file is Checked-in. Check-out the file from Source Control and try this operation again.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CheckedInFileException(FileInfo file) 
            : base(String.Format(ErrorMessage, file.Name))
        {
        }

        /// <summary>
        /// Instantiates an new instance and supplies a message and inner exception.
        /// </summary>
        /// <param name="message">Message associated with generated exception.</param>
        /// <param name="e">Inner exception.</param>
        public CheckedInFileException(string message, Exception e)
            : base(message, e)
        {
        }

        /// <summary>
        /// Instantiates an new instance and supplies a message.
        /// </summary>
        /// <param name="message">Message associated with generated exception.</param>
        public CheckedInFileException(string message)
            : base(message)
        {
        }

        public CheckedInFileException(Exception e)
            : base(e.Message, e)
        {
        }
    }
}
