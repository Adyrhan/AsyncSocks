using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IInboundMessageSpooler<T> : IThreadRunner
    {
        BlockingCollection<ReadResult<T>> Queue { get; }
        event PeerDisconnected<T> OnPeerDisconnected;
    }
}
