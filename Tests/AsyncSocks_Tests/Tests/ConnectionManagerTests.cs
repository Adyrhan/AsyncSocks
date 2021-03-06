﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class ConnectionManagerTests
    {
        private IConnectionManager<byte[]> connManager;
        private Dictionary<IPEndPoint, IAsyncClient<byte[]>> dict;

        [TestInitialize]
        public void BeforeEach()
        {
            dict = new Dictionary<IPEndPoint, IAsyncClient<byte[]>>();
            connManager = new ConnectionManager<byte[]>(dict);
        }

        [TestMethod]
        public void AddShouldAddPeerConnectionObjectToItsDictionary()
        {
            var tcpClientMock = new Mock<ITcpClient>();
            var socketMock = new Mock<ISocket>();
            var peerConnectionMock = new Mock<IAsyncClient<byte[]>> ();
            
            var endPoint = new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80);

            peerConnectionMock.Setup(x => x.Start()).Verifiable();
            peerConnectionMock.Setup(x => x.RemoteEndPoint).Returns(endPoint).Verifiable(); 

            connManager.Add(peerConnectionMock.Object);

            peerConnectionMock.Verify();

            Assert.AreEqual(endPoint, dict[endPoint].RemoteEndPoint); 
        }

        [TestMethod]
        public void CloseAllConnectionsShouldCloseConnections()
        {
            var connections = new List<Mock<IAsyncClient<byte[]>>> ();
            for(int i = 0; i < 10; i++)
            {
                var conn = new Mock<IAsyncClient<byte[]>> ();
                var messagePollerMock = new Mock<IMessagePoller<byte[]>>();
                
                conn.Setup(x => x.RemoteEndPoint).Returns(new IPEndPoint(IPAddress.Parse("80.80.80."+i.ToString()), 80));
                conn.Setup(x => x.Close()).Verifiable();

                connections.Add(conn);

                connManager.Add(conn.Object);
            }
            connManager.CloseAllConnections();

            foreach(Mock<IAsyncClient<byte[]>> conn in connections)
            {
                conn.Verify();
            }
        }

        [TestMethod]
        public void OnNewClientMessageReceivedCallbacksShouldBeCalledWhenEventIsFiredByAPeerConnectionInstance()
        {
            var callbackCalledEvent = new AutoResetEvent(false);
            var peerConnectionMock = new Mock<IAsyncClient<byte[]>> ();

            peerConnectionMock.Setup(x => x.RemoteEndPoint).Returns(new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80));

            connManager.Add(peerConnectionMock.Object);

            NewMessageReceived<byte[]> callback = (object sender, NewMessageReceivedEventArgs<byte[]> e) => callbackCalledEvent.Set();

            connManager.OnNewMessageReceived += callback;

            var ev = new NewMessageReceivedEventArgs<byte[]>(peerConnectionMock.Object, Encoding.ASCII.GetBytes("This is a test!"));

            peerConnectionMock.Raise(x => x.OnNewMessage += null, ev);

            bool called = callbackCalledEvent.WaitOne(2000);

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void ShouldFireOnPeerDisconnectedEventWhenPeerConnectionDoesIt()
        {
            var callbackCalledEvent = new AutoResetEvent(false);

            connManager.OnPeerDisconnected += (object sender, PeerDisconnectedEventArgs<byte[]> e) => callbackCalledEvent.Set();

            var peerConnectionMock = new Mock<IAsyncClient<byte[]>> ();
            peerConnectionMock.Setup(x => x.RemoteEndPoint).Returns(new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80));
            connManager.Add(peerConnectionMock.Object);

            var ev = new PeerDisconnectedEventArgs<byte[]>(peerConnectionMock.Object);

            peerConnectionMock.Raise(x => x.OnPeerDisconnected += null, ev);

            Assert.IsTrue(callbackCalledEvent.WaitOne(2000));
        }

        [TestMethod]
        public void ShouldRemovePeerConnectionFromDictWhenInstanceFiresOnPeerDisconnectedEvent()
        {
            var peerConnectionMock = new Mock<IAsyncClient<byte[]>> ();
            var endPoint = new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80);
            peerConnectionMock.Setup(x => x.RemoteEndPoint).Returns(endPoint);
            connManager.Add(peerConnectionMock.Object);

            Assert.IsTrue(dict.ContainsKey(endPoint));

            var ev = new PeerDisconnectedEventArgs<byte[]>(peerConnectionMock.Object);

            peerConnectionMock.Raise(x => x.OnPeerDisconnected += null, ev);

            Assert.IsFalse(dict.ContainsKey(endPoint));
        }
    }
}
