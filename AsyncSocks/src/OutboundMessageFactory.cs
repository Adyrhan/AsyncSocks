using System;
using System.Net.Sockets;

namespace AsyncSocks
{
    public class OutboundMessageFactory<T> : IOutboundMessageFactory<T>
    {
        public OutboundMessageFactory() {}

        public OutboundMessage<T> Create(T msg, Action<bool, SocketException> callback)
        {
            return new OutboundMessage<T>(msg, callback);
        }
    }
}