using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IConnectionManager
    {
        void CloseAllConnetions();

        event NewMessageReceived OnNewMessageReceived;
        event PeerDisconnected OnPeerDisconnected;

        void Add(IAsyncClient peerConnection);
    }
}
