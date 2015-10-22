using System;

namespace AsyncSocks
{
    /// <summary>
    /// Delegate for client disconnection events.
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    /// <param name="sender">Object that fired the event.</param>
    /// <param name="e">Event args.</param>
    public delegate void PeerDisconnected<T>(object sender, PeerDisconnectedEventArgs<T> e);

    /// <summary>
    /// Event args for the delegate PeerDisconnected. See <see cref="PeerDisconnected{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    public class PeerDisconnectedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The client that disconnected.
        /// </summary>
        public IAsyncClient<T> Peer { get; }

        public PeerDisconnectedEventArgs(IAsyncClient<T> peer)
        {
            Peer = peer;
        }
    }
}