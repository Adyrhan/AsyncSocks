using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IConnectionManager
    {
        void Add(ITcpClient tcpClient);
        void Add(IInboundMessageSpooler inbound, IOutboundMessageSpooler outbound, IMessagePoller poller, ITcpClient client);
        void CloseAllConnetions();

        event NewClientMessageDelegate OnNewClientMessageReceived;
    }

    public delegate void NewClientMessageDelegate(IPeerConnection sender, byte[] message);
}
