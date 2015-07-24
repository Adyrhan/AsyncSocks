using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public interface IAsyncServer
    {
        event NewClientMessageReceived OnNewMessageReceived;
        event PeerDisconnected OnPeerDisconnected;
        event NewPeerConnectionDelegate OnNewClientConnected;

        void Start();
        void Stop();
    }
}
