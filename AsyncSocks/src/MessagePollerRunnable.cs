using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    public class MessagePollerRunnable : IMessagePollerRunnable, IDisposable
    {
        private BlockingCollection<NetworkMessage> queue;
        private AutoResetEvent startedEvent = new AutoResetEvent(false);
        private bool shouldStop;
        private bool running;

        public MessagePollerRunnable(BlockingCollection<NetworkMessage> queue)
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

        public event NewClientMessageReceived OnNewMessageReceived;

        public void Poll()
        {
            try
            {
                NetworkMessage message = queue.Take();
                if (OnNewMessageReceived != null)
                {
                    OnNewMessageReceived(message.Sender, message.Message);
                }
            }
            catch (ThreadInterruptedException)
            {
                
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
