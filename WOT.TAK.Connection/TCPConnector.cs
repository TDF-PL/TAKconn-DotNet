using System.Net.Sockets;
using System.Xml.Serialization;
using WOT.TAK.Connection.DTOs;

namespace WOT.TAK.Connection;

internal sealed class TcpConnector(Url url, Port port) : ITakServerConnector
{
    private readonly TcpClient socket = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private Stream? stream;
    private IEventReceiver? handler;
    private ResponseListener? responseListener;
    private volatile bool closed;

    public void Send(Event anEvent)
    {
        if (this.socket.Connected is false)
        {
            throw new InvalidOperationException("Connector is not connected.");
        }

        var serializer = new XmlSerializer(typeof(Event));
        serializer.Serialize(this.stream!, anEvent);
        this.stream!.Flush();
    }

    public void Connect(IEventReceiver aHandler)
    {
        this.handler = aHandler;
        if (this.socket.Connected)
        {
            return;
        }

        this.socket.Connect(url.AsString(), port.AsInt());

        this.stream = this.socket.GetStream();
        this.responseListener = ResponseListener.Create(this.stream, this.handler, this.cancellationTokenSource);
    }

    public void Close()
    {
        if (this.closed)
        {
            return;
        }

        this.cancellationTokenSource.Cancel();
        this.responseListener?.Dispose();
        this.socket.Close();
        this.stream?.Close();
        this.closed = true;
    }

    public void Dispose()
    {
        this.Close();
        this.cancellationTokenSource.Dispose();
    }
}