using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    /// <summary>
    /// Runnable used by the thread run by MessagePoller. See <see cref="MessagePoller{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type for the message object associated with the protocol that the AsyncClient instance is using.</typeparam>
    public class MessagePollerRunnable<T> : IMessagePollerRunnable<T>, IDisposable
    {
        private BlockingCollection<ReadResult<T>> queue;
        private AutoResetEvent startedEvent = new AutoResetEvent(false);
        private bool shouldStop;
        private bool running;

        public event NewMessageReceived<T> OnNewMessageReceived;
        public event ReadErrorEventHandler OnReadError;

        public MessagePollerRunnable(BlockingCollection<ReadResult<T>> queue)
        {
            this.queue = queue;
        }

        public void Run()
        {
            running = true;
            startedEvent.Set();
            while (!shouldStop)
            {
                Poll();
            }
            running = false;
        }

        public void Stop()
        {
            shouldStop = true;
        }

        public bool IsRunning
        {
            get { return running; }
        }

        public bool WaitStarted()
        {
            return startedEvent.WaitOne(2000);
        }

        

        public void Poll() // FIXME: This probably needs to take into account the error property of the ReadResult object and fire an error event instead of a new message event
        {
            try
            {
                ReadResult<T> message = queue.Take();
                
                if (message.Error != null)
                {
                    RaiseReadError(message.Error);
                }
                else
                {
                    RaiseMessageReceived(message.Message);
                }
            }
            catch (ThreadInterruptedException)
            {
                
            }
        }

        protected void RaiseMessageReceived(T message)
        {
            var onNewMessageReceived = OnNewMessageReceived;
            if (onNewMessageReceived != null)
            {
                onNewMessageReceived(this, new NewMessageReceivedEventArgs<T>(null, message));
            }
        }

        protected void RaiseReadError(Exception error)
        {
            var onReadError = OnReadError;
            if (onReadError != null)
            {
                onReadError(this, new ReadErrorEventArgs(error));
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar llamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    startedEvent.Dispose();
                    queue.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
