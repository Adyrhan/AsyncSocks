using System;
using System.Net.Sockets;

namespace AsyncSocks
{
    public class OutboundMessageFactory : IOutboundMessageFactory
    {
        public OutboundMessageFactory() {}

        public OutboundMessage Create(byte[] msg, Action<bool, SocketException> callback)
        {
            return new OutboundMessage(msg, callback);
        }
    }
}