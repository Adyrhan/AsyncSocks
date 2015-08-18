using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class OutboundMessage
    {
        public OutboundMessage(byte[] messageBytes, Action<bool, SocketException> callback)
        {
            MessageBytes = messageBytes;
            Callback = callback;
        }

        public byte[] MessageBytes { get; }
        public Action<bool, SocketException> Callback { get; }
    }
}
