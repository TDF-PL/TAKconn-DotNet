using Microsoft.Extensions.Configuration;
using WOT.TAK.Connection.TestCommon.Configuration;
using static LanguageExt.Prelude;

namespace WOT.TAK.Connection.TestApp;

internal static class Settings
{
    public static ConnectorSettings Load(Protocol protocol)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .Build().GetSection("TestServers").Get<TestServer[]>();

        var connectorSettings = Optional(configuration)
            .IfNone(() => throw new KeyNotFoundException("No servers found in configuration."))
            .Find(server => server.SupportedProtocols.Contains(protocol))
            .Map(server => server.ConnectorSettings)
            .IfNone(() => throw new KeyNotFoundException($"No {protocol} server found in configuration."));

        return connectorSettings;
    }
}