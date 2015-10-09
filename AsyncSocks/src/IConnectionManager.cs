using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IConnectionManager<T>
    {
        void CloseAllConnections();

        event NewMessageReceived<T> OnNewMessageReceived;
        event PeerDisconnected<T> OnPeerDisconnected;

        void Add(IAsyncClient<T> peerConnection);
    }
}
