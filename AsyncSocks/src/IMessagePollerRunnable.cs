using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IMessagePollerRunnable<T> : IRunnable
    {
        event NewMessageReceived<T> OnNewMessageReceived;

        void Poll();
    }
}
