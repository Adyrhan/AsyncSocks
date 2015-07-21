using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    public class OutboundMessageSpoolerRunnable : IOutboundMessageSpoolerRunnable
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
                tcpClient.Write(message, 0, message.Length);
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
    }
}
