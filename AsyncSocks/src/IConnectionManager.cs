using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IConnectionManager
    {
        void CloseAllConnetions();

        event NewClientMessageDelegate OnNewClientMessageReceived;

        void Add(IPeerConnection peerConnection);
    }

    public delegate void NewClientMessageDelegate(IPeerConnection sender, byte[] message);
}
