using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class NetworkMessage<T>
    {
        public IAsyncClient<T> Sender { get; }
        public Exception Error { get; }
        public T Message { get; }

        public NetworkMessage(IAsyncClient<T> sender, T message, Exception error)
        {
            Error = error;
            Message = message;
            Sender = sender;
        }

        public NetworkMessage(T message)
        {
            Message = message;
        }

        public NetworkMessage(Exception error)
        {
            Error = error;
        }

        public NetworkMessage(IAsyncClient<T> sender, T message) : this(sender, message, null) { }
        public NetworkMessage(IAsyncClient<T> sender, Exception error) : this(sender, default(T), error) { }
    }
}
