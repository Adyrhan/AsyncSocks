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
        void Connect();
        void Connect(IPEndPoint remoteEndPoint);
        void Close();

        EndPoint RemoteEndPoint { get; }
        bool IsActive();

        event NewMessageReceived OnNewMessageReceived;
        event PeerDisconnected OnPeerDisconnected;

        ITcpClient TcpClient { get; }
    }
    
}
