using WOT.TAK.Connection.DTOs;

namespace WOT.TAK.Connection;

public interface ITakServerConnector : IDisposable
{
    public void Send(Event anEvent);

    public void Connect(IEventReceiver aHandler);

    public void Close();
}