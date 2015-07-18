using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class MessagePoller : ThreadRunner, IMessagePoller
    {
        private IMessagePollerRunnable runnable;

        public MessagePoller(IMessagePollerRunnable runnable) : base(runnable)
        {
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

        public static IMessagePoller Create(BlockingCollection<NetworkMessage> queue)
        {
            return new MessagePoller(new MessagePollerRunnable(queue));
        }


        public BlockingCollection<NetworkMessage> Queue
        {
            get { throw new NotImplementedException(); }
        }
    }
}
