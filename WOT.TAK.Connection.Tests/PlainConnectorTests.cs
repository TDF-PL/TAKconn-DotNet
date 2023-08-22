using NUnit.Framework;

namespace WOT.TAK.Connection.Tests;

[TestFixture]
public class PlainConnectorTests
{
    public ConnectorFactory createFactory(String output)
    {
        File.Delete(output);
        Directory.CreateDirectory(output);
        var settings = new ConnectorSettings
        {
            TrustStorePath = "cert-test/truststore-int-ca.p12",
            TrustStorePassword = "atakatak",
            KeyStorePath = "cert-test/charlie.p12",
            KeyStorePassword = "atakatak",
            ServerUrl = "tak-dev.1gs20.net",
            ServerPort = "8089",
            CotResponsesDirPath = output,
            CertificateVerification = true
        };
        return new ConnectorFactory(settings);
    }

    [Test]
    public void testDevServerMASSL()
    {
        // given
        String tmpdir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        ConnectorFactory factory = createFactory(tmpdir);
        // when
        TAKServerConnector connector = factory.GetSSLConnector();
        connector.Connect();
        Thread.Sleep(2000);
        connector.Close();
        // then
        var files = Directory.GetFiles(tmpdir);
        Assert.IsTrue(files.Length > 0);
        Assert.IsTrue(files[0].EndsWith("cot"));
        var content = File.ReadAllText(Path.Combine(tmpdir, files[0]));
        Assert.IsTrue(content.StartsWith("<?xml"));
        Assert.IsTrue(content.Contains("<event"));
        Assert.IsTrue(content.Contains("TakServerVersionInfo"));
        Assert.IsTrue(content.Trim().EndsWith("</event>"));
    }
}