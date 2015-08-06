using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    public class OutboundMessageSpoolerRunnable : IOutboundMessageSpoolerRunnable, IDisposable
    {
        private ITcpClient tcpClient;
        private BlockingCollection<byte[]> queue;
        private AutoResetEvent startEvent = new AutoResetEvent(false);
        private bool running;
        private bool shouldStop;

        public OutboundMessageSpoolerRunnable(ITcpClient tcpClient, BlockingCollection<byte[]> queue)
        {
            this.tcpClient = tcpClient;
            this.queue = queue;
        }

        public void Spool()
        {
            try
            {
                byte[] message = queue.Take();
                byte[] size = BitConverter.GetBytes(message.Length);

                int totalSize = size.Length + message.Length;

                byte[] packetBytes = new byte[totalSize];

                for (int i = 0; i < totalSize; i++)
                {
                    if (i < size.Length)
                    {
                        packetBytes[i] = size[i];
                    }
                    else
                    {
                        packetBytes[i] = message[i - size.Length];
                    }
                }

                tcpClient.Write(packetBytes, 0, totalSize);
            }
            catch (ThreadInterruptedException)
            {

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

        public void Dispose()
        {
            ((IDisposable)queue).Dispose();
            startEvent.Dispose();
        }
    }
}
