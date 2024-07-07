namespace WOT.TAK.Connection;

public sealed class ConnectorFactory(ConnectorSettings settings)
{
    public ITakServerConnector CreateSslConnector()
    {
        return new SslConnector(
            settings.ServerUrl,
            settings.ServerPort,
            settings.KeyStorePath,
            settings.KeyStorePassword);
    }

    public ITakServerConnector CreateTakServerConnector()
    {
        return new CredentialsConnector(
            settings.ServerUrl,
            settings.ServerPort,
            settings,
            settings.Login,
            settings.Password);
    }

    public ITakServerConnector CreateUdpConnector()
    {
        return new UdpConnector(
            settings.ServerUrl,
            settings.ServerPort);
    }

    public ITakServerConnector CreateTcpConnector()
    {
        return new TcpConnector(
            settings.ServerUrl,
            settings.ServerPort);
    }
}