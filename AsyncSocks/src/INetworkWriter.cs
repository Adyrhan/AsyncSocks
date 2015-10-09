using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    interface INetworkWriter<T>
    {
        void Write(T message);
        ITcpClient Client { get; }
    }
}
