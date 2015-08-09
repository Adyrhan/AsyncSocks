using System;

namespace AsyncSocks
{
    public delegate void PeerDisconnected(object sender, PeerDisconnectedEventArgs e);

    public class PeerDisconnectedEventArgs : EventArgs
    {
        public IAsyncClient Peer { get; }

        public PeerDisconnectedEventArgs(IAsyncClient peer)
        {
            Peer = peer;
        }
    }
}