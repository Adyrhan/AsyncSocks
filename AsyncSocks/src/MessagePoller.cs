using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class MessagePoller<T> : ThreadRunner, IMessagePoller<T>
    {
        private IMessagePollerRunnable<T> runnable;

        public MessagePoller(IMessagePollerRunnable<T> runnable) : base(runnable)
        {
            this.runnable = runnable;
            runnable.OnNewMessageReceived += runnable_OnNewMessageReceived;
        }

        private void runnable_OnNewMessageReceived(object sender, NewMessageReceivedEventArgs<T> e)
        {
            if (OnNewClientMessageReceived != null)
            {
                OnNewClientMessageReceived(this, e);
            }
        }

        public event NewMessageReceived<T> OnNewClientMessageReceived;

        public static IMessagePoller<T> Create(BlockingCollection<ReadResult<T>> queue)
        {
            return new MessagePoller<T>(new MessagePollerRunnable<T>(queue));
        }
    }
}
