namespace AsyncSocks
{
    /// <summary>
    /// Interface for the implementation of a NetworkWriter.
    /// The implementation of this interface has the responsibility to take a message and write its bytes to the network, following a protocol.
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    public interface INetworkWriter<T>
    {
        /// <summary>
        /// Takes the message passed as argument and sends its bytes to the client on the other side of the network.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        void Write(T message);
        /// <summary>
        /// Instance of the wrapped TcpClient object associated to the client that the messages will be sent.
        /// </summary>
        ITcpClient Client { get; }
    }
}
