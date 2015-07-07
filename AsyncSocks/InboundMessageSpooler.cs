using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace AsyncSocks
{
    public class InboundMessageSpooler : IRunnable
    {
        private INetworkMessageReader networkMessageReader;
        private BlockingCollection<byte[]> queue;
        private AutoResetEvent startedEvent = new AutoResetEvent(false);
        private bool shouldStop;
        private bool running;

        public InboundMessageSpooler(INetworkMessageReader networkMessageReader, BlockingCollection<byte[]> queue)
        {
            // TODO: Complete member initialization
            this.networkMessageReader = networkMessageReader;
            this.queue = queue;
        }

        public void Spool()
        {
            byte[] message = networkMessageReader.Read();
            queue.Add(message);
        }

        public void Run()
        {
            running = true;
            startedEvent.Set();
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
    }
}
