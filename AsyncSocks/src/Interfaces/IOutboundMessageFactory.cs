using System;
using System.Net.Sockets;

namespace AsyncSocks
{
    public interface IOutboundMessageFactory<T>
    {
        OutboundMessage<T> Create(T msg, Action<bool, SocketException> callback);
    }
}