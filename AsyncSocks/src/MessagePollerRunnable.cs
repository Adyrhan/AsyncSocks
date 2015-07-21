using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    public class MessagePollerRunnable : IMessagePollerRunnable
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

        public event NewClientMessageDelegate OnNewMessageReceived;

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
    }
}
