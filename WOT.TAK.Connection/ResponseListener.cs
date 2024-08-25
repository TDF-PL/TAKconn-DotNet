using System.Xml;
using System.Xml.Serialization;
using WOT.TAK.Connection.DTOs;

namespace WOT.TAK.Connection;

internal class ResponseListener : IDisposable
{
    private readonly Task task;

    private ResponseListener(Task task) => this.task = task;

    public static ResponseListener Create(
        Stream stream,
        IEventReceiver eventReceiver,
        CancellationTokenSource cancellationTokenSource)
    {
        return new ResponseListener(
            Task.Run(() => Process(stream, eventReceiver, cancellationTokenSource), cancellationTokenSource.Token));
    }

    public void Dispose()
    {
        try
        {
            this.task.Wait();
        }
        catch (AggregateException)
        {
        }
        catch (TaskCanceledException)
        {
        }
    }

    private static async Task Process(
        Stream stream,
        IEventReceiver eventReceiver,
        CancellationTokenSource cancellationTokenSource)
    {
        var serializer = new XmlSerializer(typeof(Event));

        while (cancellationTokenSource.IsCancellationRequested is false)
        {
            try
            {
                var xmlEvent = await EventReader.ReadFromAsync(
                        stream,
                        cancellationTokenSource.Token)
                    .ConfigureAwait(false);

                using var stringReader = new StringReader(xmlEvent!);
                using var xmlReader = XmlReader.Create(stringReader);
                var anEvent = (Event)serializer.Deserialize(xmlReader)!;

                eventReceiver.Receive(anEvent);
            }
            catch (InvalidOperationException e)
            {
                eventReceiver.OnError(e.ToString());
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }
}