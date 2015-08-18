using System;
using System.Net.Sockets;

namespace AsyncSocks
{
    public interface IOutboundMessageFactory
    {
        OutboundMessage Create(byte[] msg, Action<bool, SocketException> callback);
    }
}