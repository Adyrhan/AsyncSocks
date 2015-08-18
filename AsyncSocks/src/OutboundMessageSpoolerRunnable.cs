using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    public class OutboundMessageSpoolerRunnable : IOutboundMessageSpoolerRunnable, IDisposable
    {
        private ITcpClient tcpClient;
        private BlockingCollection<OutboundMessage> queue;
        private AutoResetEvent startEvent = new AutoResetEvent(false);
        private bool running;
        private bool shouldStop;

        public OutboundMessageSpoolerRunnable(ITcpClient tcpClient, BlockingCollection<OutboundMessage> queue)
        {
            this.tcpClient = tcpClient;
            this.queue = queue;
        }

        public void Spool()
        {
            try
            {
                OutboundMessage message = queue.Take();
                byte[] messageBytes = message.MessageBytes;

                byte[] size = BitConverter.GetBytes(messageBytes.Length);
                int totalSize = size.Length + messageBytes.Length;

                byte[] packetBytes = new byte[totalSize];

                for (int i = 0; i < totalSize; i++)
                {
                    if (i < size.Length)
                    {
                        packetBytes[i] = size[i];
                    }
                    else
                    {
                        packetBytes[i] = messageBytes[i - size.Length];
                    }
                }

                SocketException e = WriteOrException(packetBytes, 0, totalSize);

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

        private SocketException WriteOrException(byte[] bytes, int offset, int length)
        {
            try
            {
                tcpClient.Write(bytes, offset, length);
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
