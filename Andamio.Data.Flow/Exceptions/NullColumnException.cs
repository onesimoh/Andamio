using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Data
{
    public class NullColumnException : Exception
    {
        public NullColumnException(string message)
            : base(message)
        {
        }
    }
}
