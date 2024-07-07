using System.Net.Sockets;
using System.Xml.Serialization;
using WOT.TAK.Connection.DTOs;

namespace WOT.TAK.Connection;

internal sealed class UdpConnector(Url url, Port port) : ITakServerConnector
{
    private readonly UdpClient socket = new();
    private IEventReceiver? takHandler;

    public void Connect(IEventReceiver aHandler)
    {
        this.takHandler = aHandler;
        this.socket.Connect(url.AsString(), port.AsInt());
    }

    public void Send(Event anEvent)
    {
        var serializer = new XmlSerializer(typeof(Event));
        using var memoryStream = new MemoryStream();
        serializer.Serialize(memoryStream, anEvent);
        var bytes = memoryStream.ToArray();

        using var udpClient = new UdpClient();
        udpClient.Send(bytes, bytes.Length);
    }

    public void Close()
    {
        this.socket.Close();
    }

    public void Dispose()
    {
        this.Close();
    }
}