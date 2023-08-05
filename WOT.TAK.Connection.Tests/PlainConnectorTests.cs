using NUnit.Framework;

namespace WOT.TAK.Connection.Tests;

[TestFixture]
public class PlainConnectorTests
{
    [SetUp]
    public void SetUp()
    {
        var settings = new ConnectorSettings
        {
            TrustStorePath = "cert-test/truststore-int-ca.p12",
            TrustStorePassword = "atakatak",
            KeyStorePath = "cert-test/charlie.p12",
            KeyStorePassword = "atakatak",
            ServerUrl = "tak-dev.1gs20.net",
            ServerPort = "8089",
            CertificateVerification = true
        };
        _connector = new ConnectorFactory(settings).GetSSLConnector();
    }

    private TAKServerConnector _connector;

    [Test]
    public void TestDevServerMASSL()
    {
        _connector.Connect();
        Assert.NotNull(_connector);
    }
}