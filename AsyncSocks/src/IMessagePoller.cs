using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IMessagePoller : IThreadRunner
    {
        event NewClientMessageDelegate OnNewClientMessageReceived;
        BlockingCollection<NetworkMessage> Queue { get; }
    }
}
