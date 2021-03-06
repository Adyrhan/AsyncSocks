﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IOutboundMessageSpooler<T> : IThreadRunner
    {
        void Enqueue(OutboundMessage<T> outboundMessage);
    }
}
