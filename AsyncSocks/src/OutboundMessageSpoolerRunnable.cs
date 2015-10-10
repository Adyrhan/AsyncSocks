using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    //FIXME: Refactor Spool source code into a new class. That code should become the NetworkMessageWriter, which will be of an implementation of a new interface INetworkWriter.
    public class OutboundMessageSpoolerRunnable<T> : IOutboundMessageSpoolerRunnable, IDisposable
    {
        private INetworkWriter<T> writer;
        private BlockingCollection<OutboundMessage<T>> queue;
        private AutoResetEvent startEvent = new AutoResetEvent(false);
        private bool running;
        private bool shouldStop;

        public OutboundMessageSpoolerRunnable(INetworkWriter<T> writer, BlockingCollection<OutboundMessage<T>> queue)
        {
            this.writer = writer;
            this.queue = queue;
        }

        public void Spool()
        {
            try
            {
                OutboundMessage<T> message = queue.Take();
                T messageBytes = message.Message;

                SocketException e = WriteOrException(message.Message);
                
                if (message.Callback != null)
                {
                    if (e != null)
                    {
                        message.Callback(false, e);
                    }
                    else
                    {
                        message.Callback(true, null);
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                // OutboundMessageSpooler stopping
            }

        }

        private SocketException WriteOrException(T message)
        {
            try
            {
                writer.Write(message);
                return null;
            }
            catch (SocketException e)
            {
                return e;
            }
        }

        public void Run()
        {
            running = true;
            startEvent.Set();
            while (!shouldStop)
            {
                Spool();
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
            return startEvent.WaitOne();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ((IDisposable)queue).Dispose();
                    startEvent.Dispose();
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
