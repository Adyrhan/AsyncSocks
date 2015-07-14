using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IMessagePollerRunnable : IRunnable
    {
        event NewClientMessageDelegate OnNewMessageReceived;
    }
}
