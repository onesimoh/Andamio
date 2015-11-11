using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Andamio.Messaging.Formatters
{
    public interface IMessageSerializer
    {
        Message Read(Stream deserializationStream);
        void Write(Stream serializationStream, Message message);
    }
}
