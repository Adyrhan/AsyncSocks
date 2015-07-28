using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsyncSocks
{
    public delegate void PeerDisconnected(IAsyncClient peer);

    public class AsyncClient : IAsyncClient
    {
        private IInboundMessageSpooler inboundSpooler;
        private IOutboundMessageSpooler outboundSpooler;
        private ITcpClient tcpClient;
        private IMessagePoller poller;

        public event NewClientMessageReceived OnNewMessageReceived;
        public event PeerDisconnected OnPeerDisconnected;

        public AsyncClient(IInboundMessageSpooler inboundSpooler, IOutboundMessageSpooler outboundSpooler, IMessagePoller poller, ITcpClient tcpClient)
        {
            this.inboundSpooler = inboundSpooler;
            this.outboundSpooler = outboundSpooler;
            this.poller = poller;
            this.tcpClient = tcpClient;
            poller.OnNewClientMessageReceived += poller_OnNewClientMessageReceived;
            inboundSpooler.OnPeerDisconnected += InboundSpooler_OnPeerDisconnected;
        }

        private void InboundSpooler_OnPeerDisconnected(IAsyncClient peer)
        {
            var onPeerDisconnected = OnPeerDisconnected;
            if (onPeerDisconnected != null)
            {
                onPeerDisconnected(this);
            }
        }

        private void poller_OnNewClientMessageReceived(IAsyncClient sender, byte[] message)
        {
            if (OnNewMessageReceived != null)
            {
                OnNewMessageReceived(this, message);
            }
        }

        public void SendMessage(byte[] messageBytes)
        {
            outboundSpooler.Enqueue(messageBytes);
        }

        public void Start()
        {
            inboundSpooler.Start();
            outboundSpooler.Start();
            poller.Start();
        }

        public void Close()
        {
            poller.Stop();
            inboundSpooler.Stop();
            outboundSpooler.Stop();

            tcpClient.Close();
        }

        public EndPoint RemoteEndPoint
        {
            get { return tcpClient.Client.RemoteEndPoint; }
        }

        public bool IsActive()
        {
            return (inboundSpooler.IsRunning() && outboundSpooler.IsRunning() && poller.IsRunning());
        }

        public ITcpClient TcpClient
        {
            get { return tcpClient; }
        }

        public static AsyncClient Create(IPEndPoint remoteIpAddress)
        {
            BaseTcpClient client = new BaseTcpClient(new TcpClient(remoteIpAddress));
            return (AsyncClient)new PeerConnectionFactory().Create(client);
        }
    }
}
