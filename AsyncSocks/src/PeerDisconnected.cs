using System;

namespace AsyncSocks
{
    public delegate void PeerDisconnected<T>(object sender, PeerDisconnectedEventArgs<T> e);

    public class PeerDisconnectedEventArgs<T> : EventArgs
    {
        public IAsyncClient<T> Peer { get; }

        public PeerDisconnectedEventArgs(IAsyncClient<T> peer)
        {
            Peer = peer;
        }
    }
}