using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    /// <summary>
    /// Message that is going to be sent to a client. Its queued by the OutboundMessageSpooler
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    public class OutboundMessage<T>
    {
        public OutboundMessage(T message, Action<bool, SocketException> callback)
        {
            Message = message;
            Callback = callback;
        }

        /// <summary>
        /// Message contents
        /// </summary>
        public T Message { get; }

        /// <summary>
        /// Callback that notifies when the message has been sent or if an error has ocurred.
        /// </summary>
        public Action<bool, SocketException> Callback { get; }
    }
}
