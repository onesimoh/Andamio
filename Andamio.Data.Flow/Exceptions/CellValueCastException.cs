using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Data
{
    public class CellValueCastException : InvalidCastException
    {
        public CellValueCastException(string message)
            : base(message)
        {
        }

        public CellValueCastException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
