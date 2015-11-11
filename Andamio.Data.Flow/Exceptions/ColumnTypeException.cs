using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Data
{
    public class ColumnTypeException : Exception
    {
        public ColumnTypeException(string message)
            : base(message)
        {
        }

        public ColumnTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
