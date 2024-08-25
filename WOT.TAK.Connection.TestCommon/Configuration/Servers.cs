using System.ComponentModel;

namespace WOT.TAK.Connection.TestCommon.Configuration;

public record TestServer
{
    static TestServer()
    {
        TypeDescriptor.AddAttributes(typeof(Url), new TypeConverterAttribute(typeof(UrlTypeConverter)));
    }

    public IReadOnlyCollection<Protocol> SupportedProtocols { get; init; } = [];

    public ConnectorSettings ConnectorSettings { get; init; } = new();
}