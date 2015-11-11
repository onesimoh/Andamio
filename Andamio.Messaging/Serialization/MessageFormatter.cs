using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Andamio.Messaging.Formatters
{
    public abstract class MessageFormatter
    {
        public abstract Message Read(Stream deserializationStream);
        public abstract void Write(Stream serializationStream, Message message);

        public T Read<T>(Stream deserializationStream) where T : Message
        {
            T message = Read(deserializationStream) as T;
            return message;
        }

        public void Write<T>(Stream serializationStream, T message) where T : Message
        {
            Write(serializationStream, message);
        }
    }
}
