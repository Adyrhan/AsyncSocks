using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace AsyncSocks
{
    /// <summary>
    /// Runnable used by the thread created by an InboundMessageSpooler. See <see cref="InboundMessageSpooler{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InboundMessageSpoolerRunnable<T> : IInboundMessageSpoolerRunnable<T>, IDisposable
    {
        private INetworkReader<T> networkReader;
        private BlockingCollection<ReadResult<T>> queue;
        private AutoResetEvent startedEvent = new AutoResetEvent(false);
        private volatile bool shouldStop;
        private volatile bool running;

        public event PeerDisconnected<T> OnPeerDisconnected;

        public InboundMessageSpoolerRunnable(INetworkReader<T> networkReader, BlockingCollection<ReadResult<T>> queue)
        {
            this.networkReader = networkReader;
            this.queue = queue;
        }

        public void Spool()
        {
            try
            {
                ReadResult<T> message = networkReader.Read();
                
                if (message == null)
                {
                    var onPeerDisconnected = OnPeerDisconnected;
                    if (onPeerDisconnected != null)
                    {
                        onPeerDisconnected(this, null);
                        shouldStop = true;
                    }
                }
                else
                {
                    queue.Add(message);
                }
            }
            catch (ThreadInterruptedException)
            {
                
            }
        }

        public void Run()
        {
            running = true;
            startedEvent.Set();
            while (!shouldStop)
            {
                Spool();
            }
            running = false;
        }

        public bool WaitStarted()
        {
            return startedEvent.WaitOne();
        }

        public void Stop()
        {
            shouldStop = true;
        }

        public bool IsRunning
        {
            get { return running; }
        }

        public BlockingCollection<ReadResult<T>> Queue
        {
            get
            {
                return queue;
            }
        }

        public INetworkReader<T> Reader
        {
            get
            {
                return networkReader;
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
                    ((IDisposable)queue).Dispose();
                    startedEvent.Dispose();
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
