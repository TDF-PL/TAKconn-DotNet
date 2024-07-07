using WOT.TAK.Connection.DTOs;

namespace WOT.TAK.Connection;

internal sealed class CredentialsConnector(
    Url serverUrl,
    Port serverPort,
    ConnectorSettings socketFactory,
    string login,
    string password)
    : ITakServerConnector
{
    public void Connect(IEventReceiver aHandler)
    {
        throw new NotImplementedException();
    }

    public void Send(Event anEvent)
    {
        throw new NotImplementedException();
    }

    public void Close()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        this.Close();
    }
}