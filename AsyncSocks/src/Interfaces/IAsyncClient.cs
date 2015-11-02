using System;
using System.Net;
using System.Net.Sockets;

namespace AsyncSocks
{
    public interface IAsyncClient<T>
    {
        void SendMessage(T message);
        void Start();
        void Connect();
        void Connect(IPEndPoint remoteEndPoint);
        void Close();

        EndPoint RemoteEndPoint { get; }
        EndPoint LocalEndPoint { get; }
        bool IsActive();

        event NewMessageReceived<T> OnNewMessage;
        event PeerDisconnected<T> OnPeerDisconnected;
        event ReadErrorEventHandler OnReadError;

        ITcpClient TcpClient { get; }
        ClientConfig ClientConfig { get; }

        void SendMessage(T message, Action<bool, SocketException> callback);
    }
    
}
