using Microsoft.Extensions.Configuration;

namespace WOT.TAK.Connection.TestApp;

internal static class Settings
{
    public static ConnectorSettings Load(UserOption userOption)
    {
        var sectionName = userOption switch
        {
            UserOption.Tcp => "TCPConnector",
            UserOption.Ssl => "SSLConnectorTakTest",
            _ => throw new ArgumentOutOfRangeException(nameof(userOption), userOption, string.Empty)
        };

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var settings = configuration.GetSection(sectionName).Get<ConnectorSettings>();
        return settings!;
    }
}