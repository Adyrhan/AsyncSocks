using System;

namespace AsyncSocks
{
    /// <summary>
    /// Delegate for the event of message received.
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    /// <param name="sender">Object that fired the event.</param>
    /// <param name="e">Event args.</param>
    public delegate void NewMessageReceived<T>(object sender, NewMessageReceivedEventArgs<T> e);

    /// <summary>
    /// Event arguments for the delegate NewMessageReceived. See <see cref="NewMessageReceived{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NewMessageReceivedEventArgs<T> : EventArgs
    {
        public NewMessageReceivedEventArgs(IAsyncClient<T> sender, T message)
        {
            Sender = sender;
            Message = message;
        }

        /// <summary>
        /// The client that sent this message.
        /// </summary>
        public IAsyncClient<T> Sender { get; }

        /// <summary>
        /// The message contents
        /// </summary>
        public T Message { get; }
    }
}