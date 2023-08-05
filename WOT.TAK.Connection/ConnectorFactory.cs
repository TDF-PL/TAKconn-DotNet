using Microsoft.Extensions.Options;

namespace WOT.TAK.Connection;

public class ConnectorFactory
{
    private readonly ConnectorSettings _settings;

    public ConnectorFactory(ConnectorSettings settings)
    {
        _settings = settings;
    }

    public TAKServerConnector GetSSLConnector()
    {
        return new CertificateConnector(
            _settings.ServerUrl,
            _settings.ServerPort,
            _settings.CotResponsesDirPath,
            _settings.KeyStorePath,
            _settings.KeyStorePassword);
    }

    public TAKServerConnector GetTAKServerConnector()
    {
        return new CredentialsConnector(
            _settings.ServerUrl,
            _settings.ServerPort,
            _settings.CotResponsesDirPath,
            new SocketFactory(_settings),
            _settings.Login,
            _settings.Password);
    }

    public TAKServerConnector GetPlainConnector()
    {
        return new PlainConnector(
            _settings.ServerUrl,
            _settings.ServerPort,
            _settings.CotResponsesDirPath,
            new SocketFactory(_settings));
    }

    public TAKServerConnector GetUDPConnector()
    {
        return new UDPConnector(
            _settings.ServerUrl,
            _settings.ServerPort);
    }

    public TAKServerConnector GetTCPConnector()
    {
        return new TCPConnector(
            _settings.ServerUrl,
            _settings.ServerPort,
            _settings.CotResponsesDirPath);
        //   new SocketFactory(_settings));
    }
}