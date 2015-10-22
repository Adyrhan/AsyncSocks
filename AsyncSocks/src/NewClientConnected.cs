using System;

namespace AsyncSocks
{
    /// <summary>
    /// Delegate used for new client connections.
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    /// <param name="sender">Object that fired the event</param>
    /// <param name="e">Event arguments</param>
    public delegate void NewClientConnected<T>(object sender, NewClientConnectedEventArgs<T> e);

    /// <summary>
    /// Event arguments for the delegate NewClientConnected. See <see cref="NewClientConnected{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    public class NewClientConnectedEventArgs<T> : EventArgs
    {
        public NewClientConnectedEventArgs(IAsyncClient<T> client)
        {
            Client = client;
        }

        /// <summary>
        /// Instance of AsyncClient associated with the new connected client.
        /// </summary>
        public IAsyncClient<T> Client { get; }
    }
}