﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface ITcpListener
    {
        ITcpClient AcceptTcpClient();
        void Stop();
        void Start();
    }

}
