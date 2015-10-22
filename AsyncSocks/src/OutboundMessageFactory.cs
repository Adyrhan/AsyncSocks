using System;
using System.Net.Sockets;

namespace AsyncSocks
{
    /// <summary>
    /// Creates OutboundMessage objects. Used mainly for testing. 
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    public class OutboundMessageFactory<T> : IOutboundMessageFactory<T>
    {
        public OutboundMessageFactory() {}

        /// <summary>
        /// Creates an instance of OutboundMessage
        /// </summary>
        /// <param name="msg">Contents of the message</param>
        /// <param name="callback">Message callback</param>
        /// <returns></returns>
        public OutboundMessage<T> Create(T msg, Action<bool, SocketException> callback)
        {
            return new OutboundMessage<T>(msg, callback);
        }
    }
}