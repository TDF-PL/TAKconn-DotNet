using System.Reflection;
using WOT.TAK.Connection;
using WOT.TAK.Connection.TestApp;
using static WOT.TAK.Connection.TestCommon.ClientEvents;

Console.WriteLine("Use:");
Console.WriteLine("1. TCP");
Console.WriteLine("2. SSL");

var input = Console.ReadLine();

if (int.TryParse(input, out var option) && option is < 1 or > 2)
{
    Console.WriteLine("Invalid input");
    return;
}

var userOption = (UserOption)option;
var settings = Settings.Load(userOption);

var alphaUid = new Uid(new Guid("d2dabd71-4239-4370-a9b5-1c74d270ce18"));
var alphaCallsign = $"{Assembly.GetExecutingAssembly().GetName().Name} - Alpha ({userOption})";

var bravoUid = new Uid(new Guid("0c24f6a9-2546-487f-a689-9dcbed6699e8"));
var bravoCallsign = $"{Assembly.GetExecutingAssembly().GetName().Name} - Bravo ({userOption})";

var factory = new ConnectorFactory(settings);

var (alpha, bravo) = userOption switch
{
    UserOption.Tcp => (factory.CreateTcpConnector(), factory.CreateTcpConnector()),
    UserOption.Ssl => (factory.CreateSslConnector(), factory.CreateSslConnector()),
    _ => throw new ArgumentOutOfRangeException(nameof(userOption), userOption, string.Empty)
};

alpha.Connect(new EventReceiver());
bravo.Connect(new EventReceiver());

var alphaNewContactAnnouncement = NewContactAnnouncement(alphaUid, alphaCallsign);
var bravoNewContactAnnouncement = NewContactAnnouncement(bravoUid, bravoCallsign);

alpha.Send(alphaNewContactAnnouncement);
bravo.Send(bravoNewContactAnnouncement);

for (var i = 0; i < 10 ; i++)
{
    var alphaChatText = $"Message from Alpha {i}";
    var alphaChatPost = ChatPost(alphaUid, alphaCallsign, alphaChatText);

    alpha.Send(alphaChatPost);

    await Task.Delay(TimeSpan.FromSeconds(3));
}

alpha.Close();
bravo.Close();