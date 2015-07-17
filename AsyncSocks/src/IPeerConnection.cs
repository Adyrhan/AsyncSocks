using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public interface IPeerConnection
    {
        void SendMessage(byte[] messageBytes);
        void Start();
        void Close();

        EndPoint RemoteEndPoint { get; }
        bool IsActive();

        event NewClientMessageDelegate OnNewMessageReceived;
    }
    
}
