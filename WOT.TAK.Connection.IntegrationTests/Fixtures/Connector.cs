using System.Reflection;
using Xunit.Sdk;

namespace WOT.TAK.Connection.IntegrationTests.Fixtures;

internal sealed class ConnectorAttribute(Type connectorType) : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        return new[]
        {
            testMethod.GetParameters()
                .Select(_ =>
                {
                    if(connectorType == typeof(TcpConnector))
                    {
                        return (ITakServerConnector)CreateTcpConnector();
                    }
                    if(connectorType == typeof(SslConnector))
                    {
                        return CreateCertificateConnector();
                    }

                    throw new ArgumentException($"Unknown connector type {connectorType}");
                })
                .ToArray()
        };
    }

    private static TcpConnector CreateTcpConnector()
    {
        return new TcpConnector(
            Url.Of("localhost"),
            Port.Of(8089));
    }

    private static SslConnector CreateCertificateConnector()
    {
        return new SslConnector(
            Url.Of("SERVER-URL"),
            Port.Of(8089),
            "Fixtures/Certificates/TakTest/zintegrowane24.p12",
            "PASSWORD");

    }
}