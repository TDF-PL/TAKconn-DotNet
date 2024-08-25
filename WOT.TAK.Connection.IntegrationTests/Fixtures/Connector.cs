using System.Reflection;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using WOT.TAK.Connection.TestCommon.Configuration;
using Xunit.Sdk;

namespace WOT.TAK.Connection.IntegrationTests.Fixtures;

internal sealed class ConnectorAttribute(Type connectorType) : DataAttribute
{
    private static readonly Option<TestServer[]> Servers = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddUserSecrets<ConnectorAttribute>()
        .Build().GetSection("TestServers").Get<TestServer[]>();

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var testServers = Servers.Match(
            Some: servers => servers,
            None: () => throw new KeyNotFoundException("No servers found in configuration."));

        return
        [
            testMethod.GetParameters()
                .Select(ITakServerConnector (_) =>
                {
                    if(connectorType == typeof(TcpConnector))
                    {
                        return CreateTcpConnector(testServers);
                    }
                    if(connectorType == typeof(SslConnector))
                    {
                        return CreateCertificateConnector(testServers);
                    }

                    throw new ArgumentException($"Unknown connector type {connectorType}");
                })
                .ToArray<object>()
        ];
    }

    private static TcpConnector CreateTcpConnector(TestServer[] servers)
    {
        var tcpServer = servers.Find(
            server => server.SupportedProtocols.Contains(Protocol.Tcp));

        return tcpServer.Match(
            Some: server => new TcpConnector(server.ConnectorSettings.ServerUrl, server.ConnectorSettings.ServerPort),
            None: () => throw new InvalidOperationException("No TCP server found in configuration."));
    }

    private static SslConnector CreateCertificateConnector(TestServer[] servers)
    {
        var sslServer = servers.Find(
            server => server.SupportedProtocols.Contains(Protocol.Ssl));

        return sslServer.Match(
            Some: server => new SslConnector(
                server.ConnectorSettings.ServerUrl,
                server.ConnectorSettings.ServerPort,
                server.ConnectorSettings.ClientCertificatePath,
                server.ConnectorSettings.ClientCertificatePassword),
            None: () => throw new InvalidOperationException("No SSL server found in configuration."));

    }
}