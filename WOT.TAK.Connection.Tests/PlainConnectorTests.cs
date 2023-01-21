using NUnit.Framework;
using WOT.TAK.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOT.TAK.Connection.Tests
{

    [TestFixture]
    public class PlainConnectorTests
    {
        private TAKServerConnector _connector;

        [SetUp]
        public void SetUp()
        {
            _connector = new ConnectorFactory(null).GetPlainConnector();
        }

        [Test]
        public void GetPlainConnector_Invalid()
        {
            _connector.Connect();
            Assert.NotNull(_connector);
        }
    }
}