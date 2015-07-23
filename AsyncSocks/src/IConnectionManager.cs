using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IConnectionManager
    {
        void CloseAllConnetions();

        event NewClientMessageReceived OnNewClientMessageReceived;

        void Add(IPeerConnection peerConnection);
    }

    public delegate void NewClientMessageReceived(IPeerConnection sender, byte[] message);
}
