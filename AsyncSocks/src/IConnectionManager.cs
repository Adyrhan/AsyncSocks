using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IConnectionManager
    {
        void Add(ITcpClient tcpClient);
        void CloseAllConnetions();
    }
}
