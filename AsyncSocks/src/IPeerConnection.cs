using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public interface IAsyncClient
    {
        void SendMessage(byte[] messageBytes);
        void Start();
        void Close();

        EndPoint RemoteEndPoint { get; }
        bool IsActive();

        event NewClientMessageReceived OnNewMessageReceived;
        event PeerDisconnected OnPeerDisconnected;

        ITcpClient TcpClient { get; }
    }
    
}
