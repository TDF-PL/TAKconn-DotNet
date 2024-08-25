using System.Collections.Concurrent;
using Xunit.Abstractions;

namespace WOT.TAK.Connection.IntegrationTests;

internal sealed class EventReceiver(ITestOutputHelper errorOutput) : IEventReceiver
{
    private readonly ConcurrentQueue<DTOs.Event> events = new();

    public IReadOnlyCollection<DTOs.Event> Events => events;

    public void Receive(DTOs.Event anEvent)
    {
        this.events.Enqueue(anEvent);
    }

    public void OnError(string message)
    {
        errorOutput.WriteLine(message);
    }
}