using WOT.TAK.Connection.DTOs;

namespace WOT.TAK.Connection.TestApp;

internal class EventReceiver : IEventReceiver
{
    public void Receive(Event anEvent)
    {
        var dumpedEvent = ObjectDumper.Dump(anEvent,  new DumpOptions()
        {
            IgnoreDefaultValues = true
        });

        Console.WriteLine("Received event:");
        Console.WriteLine(dumpedEvent);
        Console.WriteLine();
    }

    public void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {message}");
        Console.ResetColor();
    }
}