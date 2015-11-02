using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public interface IAsyncServer<T>
    {
        IConnectionManager<T> ConnectionManager { get; }
        ClientConfig ClientConfig { get; }

        event NewMessageReceived<T> OnNewMessageReceived;
        event PeerDisconnected<T> OnPeerDisconnected;
        event NewClientConnected<T> OnNewClientConnected;

        void Start();
        void Stop();
    }
}
