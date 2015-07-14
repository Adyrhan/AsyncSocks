using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class MessagePoller : ThreadRunner, IMessagePoller
    {
        private BlockingCollection<byte[]> queue;
        private IMessagePollerRunnable runnable;

        public MessagePoller(IMessagePollerRunnable runnable, BlockingCollection<byte[]> queue) : base(runnable)
        {
            this.queue = queue;
            this.runnable = runnable;
            runnable.OnNewMessageReceived += runnable_OnNewMessageReceived;
        }

        private void runnable_OnNewMessageReceived(IPeerConnection sender, byte[] message)
        {
            if (OnNewClientMessageReceived != null)
            {
                OnNewClientMessageReceived(sender, message);
            }
        }

        public event NewClientMessageDelegate OnNewClientMessageReceived;
    }
}
