using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Data
{
    public class FormValidationException : ApplicationException
    {
        public FormValidationException()
            : base()
        {
        }

        public FormValidationException(string message)
            : base(message)
        {
        }

        public FormValidationException(string message, IEnumerable<KeyValuePair<string, string>> validationErrors)
            : base(message)
        {
            if (validationErrors.Any())
            {
                ValidationErrors.AddRange(validationErrors);
            }
        }

        public readonly Dictionary<string, string> ValidationErrors = new Dictionary<string, string>();
    }
}
