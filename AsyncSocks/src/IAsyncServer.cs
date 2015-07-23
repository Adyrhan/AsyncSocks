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
        void Start();
        void Stop();
    }
}
