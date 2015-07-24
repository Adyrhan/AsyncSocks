using System;
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
        private IConnectionManager connManager;
        private Dictionary<IPEndPoint, IPeerConnection> dict;

        [TestInitialize]
        public void BeforeEach()
        {
            dict = new Dictionary<IPEndPoint, IPeerConnection>();
            connManager = new ConnectionManager(dict);
        }

        [TestMethod]
        public void AddShouldAddPeerConnectionObjectToItsDictionary()
        {
            var tcpClientMock = new Mock<ITcpClient>();
            var socketMock = new Mock<ISocket>();
            var peerConnectionMock = new Mock<IPeerConnection>();
            
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
            var connections = new List<Mock<IPeerConnection>>();
            for(int i = 0; i < 10; i++)
            {
                var conn = new Mock<IPeerConnection>();
                var messagePollerMock = new Mock<IMessagePoller>();
                
                conn.Setup(x => x.RemoteEndPoint).Returns(new IPEndPoint(IPAddress.Parse("80.80.80."+i.ToString()), 80));
                conn.Setup(x => x.Close()).Verifiable();

                connections.Add(conn);

                connManager.Add(conn.Object);
            }
            connManager.CloseAllConnetions();

            foreach(Mock<IPeerConnection> conn in connections)
            {
                conn.Verify();
            }
        }

        [TestMethod]
        public void OnNewClientMessageReceivedCallbacksShouldBeCalledWhenEventIsFiredByAPeerConnectionInstance()
        {
            var callbackCalledEvent = new AutoResetEvent(false);
            var peerConnectionMock = new Mock<IPeerConnection>();

            peerConnectionMock.Setup(x => x.RemoteEndPoint).Returns(new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80));

            connManager.Add(peerConnectionMock.Object);

            NewClientMessageReceived callback = delegate(IPeerConnection sender, byte[] message)
            {
                callbackCalledEvent.Set();
            };

            connManager.OnNewClientMessageReceived += callback;

            peerConnectionMock.Raise(x => x.OnNewMessageReceived += null, peerConnectionMock.Object, Encoding.ASCII.GetBytes("This is a test!"));

            bool called = callbackCalledEvent.WaitOne(2000);

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void ShouldFireOnPeerDisconnectedEventWhenPeerConnectionDoesIt()
        {
            var callbackCalledEvent = new AutoResetEvent(false);

            connManager.OnPeerDisconnected += delegate (IPeerConnection peer)
            {
                callbackCalledEvent.Set();
            };

            var peerConnectionMock = new Mock<IPeerConnection>();
            peerConnectionMock.Setup(x => x.RemoteEndPoint).Returns(new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80));
            connManager.Add(peerConnectionMock.Object);

            peerConnectionMock.Raise(x => x.OnPeerDisconnected += null, peerConnectionMock.Object);

            Assert.IsTrue(callbackCalledEvent.WaitOne(2000));
        }

        [TestMethod]
        public void ShouldRemovePeerConnectionFromDictWhenInstanceFiresOnPeerDisconnectedEvent()
        {
            var peerConnectionMock = new Mock<IPeerConnection>();
            var endPoint = new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80);
            peerConnectionMock.Setup(x => x.RemoteEndPoint).Returns(endPoint);
            connManager.Add(peerConnectionMock.Object);

            Assert.IsTrue(dict.ContainsKey(endPoint));

            peerConnectionMock.Raise(x => x.OnPeerDisconnected += null, peerConnectionMock.Object);

            Assert.IsFalse(dict.ContainsKey(endPoint));
        }
    }
}
