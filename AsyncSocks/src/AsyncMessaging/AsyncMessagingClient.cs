using AsyncSocks.AsyncMessaging.Exceptions;
using System;
using System.Net.Sockets;


namespace AsyncSocks.AsyncMessaging
{
    namespace Exceptions
    {
        /// <summary>
        /// Exceptions used by this library.
        /// </summary>
        [System.Runtime.CompilerServices.CompilerGenerated]
        class NamespaceDoc { }

        /// <summary>
        /// This exception is thrown by AsyncMessagingClient when the message to send is too big
        /// </summary>
        [Serializable]
        public class MessageTooBigException : Exception
        {
            public MessageTooBigException(string message) : base(message)
            {
            }
        }
    }

    /// <summary>
    /// This subclass of AsyncClient will send binary messages preceded by 4 bytes that indicates the size of the message.
    /// If you want to limit the message size, change the value for the key "MaxMessageSize" to the desired size in AsyncMessagingClientConfig
    /// </summary>
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

        /// <summary>
        /// Sends binary messages to the client.
        /// </summary>
        /// <param name="message">binary data to send</param>
        /// <param name="callback"><inheritdoc/></param>
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
