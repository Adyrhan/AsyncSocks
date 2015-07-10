using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IClientConnectionAgent
    {
        event NewClientConnectionDelegate OnNewClientConnection;
    }
}
