using WOT.TAK.Connection.DTOs;

namespace WOT.TAK.Connection;

public interface IEventReceiver
{
    public void Receive(Event anEvent);

    public void OnError(string message);
}