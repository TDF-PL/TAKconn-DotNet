namespace WOT.TAK.Connection;

internal class CredentialsConnector : TAKServerConnector
{
    private string cotResponsesDirPath;
    private string login;
    private string password;
    private string serverPort;
    private string serverUrl;
    private SocketFactory socketFactory;

    public CredentialsConnector(string serverUrl, string serverPort, string cotResponsesDirPath,
        SocketFactory socketFactory, string login, string password)
    {
        this.serverUrl = serverUrl;
        this.serverPort = serverPort;
        this.cotResponsesDirPath = cotResponsesDirPath;
        this.socketFactory = socketFactory;
        this.login = login;
        this.password = password;
    }

    public void Close()
    {
        throw new NotImplementedException();
    }

    public void Connect()
    {
        throw new NotImplementedException();
    }

    public void SendFile(string path)
    {
        throw new NotImplementedException();
    }
}