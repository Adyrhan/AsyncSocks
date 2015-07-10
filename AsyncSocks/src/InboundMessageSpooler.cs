using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class InboundMessageSpooler : ThreadRunner
    {
        public static InboundMessageSpooler Create(ITcpClient tcpClient)
        {
            //NetworkMessageReader reader
            //InboundMessageSpoolerRunnable runnable = new InboundMessageSpoolerRunnable(reader, queue);
            //return new InboundMessageSpooler();

            throw new NotImplementedException();
        }

        public InboundMessageSpooler(IInboundMessageSpoolerRunnable runnable) : base(runnable) { }

    }
}
