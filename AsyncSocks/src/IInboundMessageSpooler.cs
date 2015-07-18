using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IInboundMessageSpooler : IThreadRunner
    {
        BlockingCollection<NetworkMessage> Queue { get; }
    }
}
