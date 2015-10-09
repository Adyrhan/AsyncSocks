using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class OutboundMessage<T>
    {
        public OutboundMessage(T message, Action<bool, SocketException> callback)
        {
            Message = message;
            Callback = callback;
        }

        public T Message { get; }
        public Action<bool, SocketException> Callback { get; }
    }
}
