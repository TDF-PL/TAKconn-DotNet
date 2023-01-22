using NUnit.Framework;

namespace WOT.TAK.Connection.Tests;

[TestFixture]
public class PlainConnectorTests
{
    [SetUp]
    public void SetUp()
    {
        _connector = new ConnectorFactory(null).GetPlainConnector();
    }

    private TAKServerConnector _connector;

    [Test]
    public void GetPlainConnector_Invalid()
    {
        _connector.Connect();
        Assert.NotNull(_connector);
    }
}