﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public interface INetworkMessageWriter : INetworkWriter<byte[]> { }
}