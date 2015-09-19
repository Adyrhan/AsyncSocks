using System;
using System.Net;
using System.Net.Sockets;

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
        EndPoint LocalEndPoint { get; }
        bool IsActive();

        event NewMessageReceived OnNewMessageReceived;
        event PeerDisconnected OnPeerDisconnected;

        ITcpClient TcpClient { get; }
        ClientConfig ClientConfig { get; }

        void SendMessage(byte[] msgBytes, Action<bool, SocketException> callback);
    }
    
}
