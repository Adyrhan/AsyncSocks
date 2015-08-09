using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsyncSocks
{
    

    public class AsyncClient : IAsyncClient
    {
        private IInboundMessageSpooler inboundSpooler;
        private IOutboundMessageSpooler outboundSpooler;
        private ITcpClient tcpClient;
        private IMessagePoller poller;

        public event NewMessageReceived OnNewMessageReceived;
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

        private void InboundSpooler_OnPeerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            var onPeerDisconnected = OnPeerDisconnected;
            if (onPeerDisconnected != null)
            {
                var ev = new PeerDisconnectedEventArgs(this);
                onPeerDisconnected(this, ev);
            }
        }

        private void poller_OnNewClientMessageReceived(object sender, NewMessageReceivedEventArgs e)
        {
            if (OnNewMessageReceived != null)
            {
                var newE = new NewMessageReceivedEventArgs(this, e.Message);
                OnNewMessageReceived(this, newE);
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
            return (AsyncClient)new AsyncClientFactory().Create(client);
        }

        public static AsyncClient Create()
        {
            BaseTcpClient client = new BaseTcpClient(new TcpClient());
            return (AsyncClient)new AsyncClientFactory().Create(client);
        }

        public void Connect()
        {
            tcpClient.Connect();
            Start();
        }

        public void Connect(IPEndPoint remoteEndPoint)
        {
            tcpClient.Connect(remoteEndPoint);
            Start();
        }
    }
}
