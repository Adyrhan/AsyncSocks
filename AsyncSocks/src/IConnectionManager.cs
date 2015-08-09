using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IConnectionManager
    {
        void CloseAllConnetions();

        event NewMessageReceived OnNewClientMessageReceived;
        event PeerDisconnected OnPeerDisconnected;

        void Add(IAsyncClient peerConnection);
    }

    public delegate void NewMessageReceived(IAsyncClient sender, byte[] message);
}
