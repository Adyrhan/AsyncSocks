using AsyncSocks.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    namespace Exceptions
    {
        [Serializable]
        public class MessageTooBigException : Exception
        {
            public MessageTooBigException(string message) : base(message)
            {
            }
        }
    }

    public class AsyncMessagingClient : AsyncClient<byte[]>
    {
        private int maxMessageSize;

        public AsyncMessagingClient
        (
            IInboundMessageSpooler<byte[]> inboundSpooler, IOutboundMessageSpooler<byte[]> outboundSpooler, IMessagePoller<byte[]> poller, IOutboundMessageFactory<byte[]> messageFactory, ITcpClient tcpClient, ClientConfig clientConfig
        ) : base(inboundSpooler, outboundSpooler, poller, messageFactory, tcpClient, clientConfig)
        {
            maxMessageSize = int.Parse(ClientConfig.GetProperty("MaxMessageSize"));
        }

        public override void SendMessage(byte[] message, Action<bool, SocketException> callback)
        {
            
            if (message.Length > maxMessageSize)
            {
                int maxSize = maxMessageSize;
                int msgSize = message.Length;
                throw new MessageTooBigException("Max size expected for outgoing messages is: " + maxSize.ToString() + " Received message of size: " + msgSize.ToString());
            }

            base.SendMessage(message, callback);
        }

    }
}
