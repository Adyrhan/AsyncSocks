using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace AsyncSocks
{
    public class InboundMessageSpoolerRunnable : IInboundMessageSpoolerRunnable, IDisposable
    {
        private INetworkMessageReader networkMessageReader;
        private BlockingCollection<NetworkMessage> queue;
        private AutoResetEvent startedEvent = new AutoResetEvent(false);
        private volatile bool shouldStop;
        private volatile bool running;

        public event PeerDisconnected OnPeerDisconnected;

        public InboundMessageSpoolerRunnable(INetworkMessageReader networkMessageReader, BlockingCollection<NetworkMessage> queue)
        {
            this.networkMessageReader = networkMessageReader;
            this.queue = queue;
        }

        public void Spool()
        {
            try
            {
                byte[] message = networkMessageReader.Read();
                
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
                    queue.Add(new NetworkMessage(null, message));
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

        public BlockingCollection<NetworkMessage> Queue
        {
            get
            {
                return queue;
            }
        }

        public INetworkMessageReader Reader
        {
            get
            {
                return networkMessageReader;
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
