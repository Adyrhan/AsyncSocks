﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class InboundMessageSpooler : ThreadRunner, IInboundMessageSpooler
    {
        public event PeerDisconnected OnPeerDisconnected;

        public static InboundMessageSpooler Create(ITcpClient tcpClient)
        {
            NetworkMessageReader reader = new NetworkMessageReader(tcpClient);
            BlockingCollection<NetworkMessage> queue = new BlockingCollection<NetworkMessage>(new ConcurrentQueue<NetworkMessage>());
            InboundMessageSpoolerRunnable runnable = new InboundMessageSpoolerRunnable(reader, queue);
            return new InboundMessageSpooler(runnable);
        }

        public InboundMessageSpooler(IInboundMessageSpoolerRunnable runnable) : base(runnable)
        {
            runnable.OnPeerDisconnected += Runnable_OnPeerDisconnected;
        }

        private void Runnable_OnPeerDisconnected(IPeerConnection peer)
        {
            var onPeerDisconnected = OnPeerDisconnected;

            if (onPeerDisconnected != null)
            {
                onPeerDisconnected(peer);
            }

        }

        public BlockingCollection<NetworkMessage> Queue
        {
            get { return ((IInboundMessageSpoolerRunnable) Runnable).Queue; }
        }
    }
}
