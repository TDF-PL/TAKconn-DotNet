using System.Globalization;
using FluentAssertions;
using WOT.TAK.Connection.IntegrationTests.Fixtures;
using WOT.TAK.Connection.TestCommon;
using Xunit.Abstractions;
using static WOT.TAK.Connection.TestCommon.ClientEvents;

namespace WOT.TAK.Connection.IntegrationTests;

public class ConnectorTests(ITestOutputHelper output)
{
    private readonly Uid alphaUid = new(new Guid("2695673f-d106-42bb-9879-f01b7f6d725e"));
    private readonly Uid bravoUid = new(new Guid("37551eca-6262-4c07-a4e8-7aa23c1314d9"));

    [Theory]
    [Connector(typeof(TcpConnector))]
    [Connector(typeof(SslConnector))]
    public async Task Can_create_two_connectors_which_can_communicate_via_TAK_server(
        ITakServerConnector alphaConnector, ITakServerConnector bravoConnector)
    {
        using var _ = alphaConnector;
        using var __ = bravoConnector;

        const string callsignPattern = "[{0}][{1}][XML] Automated Integration Tests";
        var alphaCallsign = string.Format(
            CultureInfo.InvariantCulture, callsignPattern, alphaConnector.GetType().Name, "Alpha");

        var bravoCallsign = string.Format(
            CultureInfo.InvariantCulture, callsignPattern, bravoConnector.GetType().Name, "Bravo");

        var alphaChatText = $"Test message {alphaUid}";
        var bravoChatText = $"Test message {bravoUid}";

        var alphaAnnouncement = NewContactAnnouncement(alphaUid, alphaCallsign);
        var alphaChatPost = ChatPost(alphaUid, alphaCallsign, alphaChatText);

        var bravoAnnouncement = NewContactAnnouncement(bravoUid, bravoCallsign);
        var bravoChatPost = ChatPost(bravoUid, bravoCallsign, bravoChatText);

        var alphaEvents = new EventReceiver(output);
        var bravoEvents = new EventReceiver(output);

        alphaConnector.Connect(alphaEvents);
        bravoConnector.Connect(bravoEvents);

        alphaConnector.Send(alphaAnnouncement);
        bravoConnector.Send(bravoAnnouncement);

        alphaConnector.Send(alphaChatPost);
        bravoConnector.Send(bravoChatPost);

        await alphaEvents.Events.Should()
            .EventuallyContain(anEvent => anEvent.Uid != null, TimeSpan.FromSeconds(10));

        await bravoEvents.Events.Should()
            .EventuallyContain(anEvent => anEvent.Uid != null, TimeSpan.FromSeconds(10));
    }
}