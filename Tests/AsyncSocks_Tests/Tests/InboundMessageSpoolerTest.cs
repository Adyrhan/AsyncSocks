﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AsyncSocks;
using System.Collections.Concurrent;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class InboundMessageSpoolerTest
    {
        [TestMethod]
        public void CreateShouldCreateInstance()
        {
            var tcpClientMock = new Mock<ITcpClient>();
            var tcpClient = tcpClientMock.Object;

            InboundMessageSpooler spooler = InboundMessageSpooler.Create(tcpClient);

            Assert.IsTrue(spooler != null && spooler is InboundMessageSpooler);
        }

        [TestMethod]
        public void ShouldBehaveLikeAThreadRunner()
        {
            var runnableMock = new Mock<IInboundMessageSpoolerRunnable>();
            var runnable = runnableMock.Object;
            var spooler = new InboundMessageSpooler(runnable);

            Assert.IsTrue(spooler is ThreadRunner);
        }

        [TestMethod]
        public void ConstructorShouldTakeRunnable()
        {
            var runnableMock = new Mock<IInboundMessageSpoolerRunnable>();
            var runnable = runnableMock.Object;
            InboundMessageSpooler spooler = new InboundMessageSpooler(runnable);
        }
    }
}