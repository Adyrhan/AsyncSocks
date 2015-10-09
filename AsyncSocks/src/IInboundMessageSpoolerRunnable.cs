using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public interface IInboundMessageSpoolerRunnable<T> : IRunnable
    {
        event PeerDisconnected<T> OnPeerDisconnected;
        void Spool();
        BlockingCollection<ReadResult<T>> Queue { get; }
        INetworkReader<T> Reader { get; }
    }
}
