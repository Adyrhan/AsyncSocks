using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    /// <summary>
    /// Runs a thread that polls a queue for messages. If one is retrieved from it, it will fire an event on the same thread. Is meant to be used
    /// with the queue used by the InboundMessageSpooler so the user can specify what to do with the incoming messages.
    /// </summary>
    /// <typeparam name="T">Type for the message object associated with the protocol that the AsyncClient instance is using.</typeparam>
    public class MessagePoller<T> : ThreadRunner, IMessagePoller<T>
    {
        /// <summary>
        /// This event fires whenever a new message has been obtained from the queue.
        /// </summary>
        public event NewMessageReceived<T> OnNewClientMessageReceived;

        /// <summary>
        /// Fires whenever a read error happened.
        /// </summary>
        public event ReadErrorEventHandler OnReadError;

        private IMessagePollerRunnable<T> runnable;

        public MessagePoller(IMessagePollerRunnable<T> runnable) : base(runnable)
        {
            this.runnable = runnable;
            runnable.OnNewMessageReceived += runnable_OnNewMessageReceived;
            runnable.OnReadError += Runnable_OnReadError;
        }

        private void Runnable_OnReadError(object sender, ReadErrorEventArgs e)
        {
            var onReadError = OnReadError;
            if (onReadError != null)
            {
                onReadError(this, e);
            }
        }

        private void runnable_OnNewMessageReceived(object sender, NewMessageReceivedEventArgs<T> e)
        {
            if (OnNewClientMessageReceived != null)
            {
                OnNewClientMessageReceived(this, e);
            }
        }

        /// <summary>
        /// Static factory that creates an instance of this class using the queue provided.
        /// </summary>
        /// <param name="queue">This is the queue that will also be used by the InbountMessageSpooler.</param>
        /// <returns>Instance of MessagePoller</returns>
        public static IMessagePoller<T> Create(BlockingCollection<ReadResult<T>> queue)
        {
            return new MessagePoller<T>(new MessagePollerRunnable<T>(queue));
        }
    }
}
