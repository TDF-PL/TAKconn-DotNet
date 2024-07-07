using System.Text;
using NUnit.Framework;

namespace WOT.TAK.Connection.Tests;

[TestFixture]
public class EventReaderTest
{
    [Test]
    public async Task TestClosedTag()
    {
        const string test = "<xml/>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var @event = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.That(@event, Is.EqualTo(test));
    }

    [Test]
    public async Task TestNoPreamble()
    {
        const string test = "<xml></xml>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var @event = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.That(@event, Is.EqualTo(test));
    }

    [Test]
    public async Task TestPreamble()
    {
        const string test = "<?xml?><xml></xml>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var @event = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.That(@event, Is.EqualTo(test));
    }

    [Test]
    public async Task TestPreambleNewLine()
    {
        const string test = "<?xml?>\n<xml></xml>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var @event = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.That(@event, Is.EqualTo(test));
    }

    [Test]
    public async Task TestTwo()
    {
        const string test = "<one></one><two></two>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var event1 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);
        var event2 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(event1, Is.EqualTo("<one></one>"));
            Assert.That(event2, Is.EqualTo("<two></two>"));
        });
    }

    [Test]
    public async Task TestNewLineTwo()
    {
        const string test = "<one></one>\n<two></two>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var event1 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);
        var event2 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(event1, Is.EqualTo("<one></one>"));
            Assert.That(event2, Is.EqualTo("<two></two>"));
        });
    }

    [Test]
    public async Task TestComplex()
    {
        const string test = "\n\n<?x ?>\n\n<one></one>\n\n<?y ?>\n<two></two>\n\n\n";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var event1 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);
        var event2 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(event1, Is.EqualTo("<?x ?>\n\n<one></one>"));
            Assert.That(event2, Is.EqualTo("<?y ?>\n<two></two>"));
        });
    }

    [Test]
    public async Task TestMix()
    {
        const string test = "\n\n<?x ?>\n\n<one></one><two  />\n\n\n";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var event1 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);
        var event2 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(event1, Is.EqualTo("<?x ?>\n\n<one></one>"));
            Assert.That(event2, Is.EqualTo("<two  />"));
        });
    }

    [Test]
    public async Task TestWhitespace()
    {
        const string test = "\n\n\n\n\n";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var @event = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.That(@event, Is.Null);
    }

    [Test]
    public async Task TestOneAndWhitespace()
    {
        const string test = "<one/>\n\n\n\n\n";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var event1 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);
        var event2 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);
        var event3 = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(event1, Is.EqualTo("<one/>"));
            Assert.That(event2, Is.Null);
            Assert.That(event3, Is.Null);
        });
    }

    [Test]
    public async Task TestClient()
    {
        const string test = """
            <event version="2.0" uid="S-1-5-21-4270384509-2508030920-1516254662-1001" type="a-f-G-U-C-I"
                    time="2023-08-22T01:40:06Z" start="2023-08-22T01:38:12.99Z"
                    stale="2023-08-22T01:44:27.99Z" how="h-g-i-g-o">
                <point lat="0" lon="0" hae="0" ce="9999999" le="9999999"/>
                <detail>
                    <takv version="4.9.0.172" platform="WinTAK-CIV" os="Microsoft Windows 10 Home"
                        device="Apple Inc. MacBookPro16,1"/>
                    <contact callsign="MUSIC" endpoint="*:-1:stcp"/>
                    <uid Droid="MUSIC"/>
                    <__group name="Green" role="Team Member"/>
                    <status battery="99"/>
                    <track course="0.00000000" speed="0.00000000"/>
                    <_flow-tags_ TAK-Server-c157ed5a69784cc3ab5e2401a5936074="2023-08-22T01:40:06Z"/>
                </detail>
            </event>
            """;

        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var @event = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.That(@event, Is.EqualTo(test));
    }

    [Test]
    public async Task TestServer()
    {
        const string test = """
            <?xml version="1.0" encoding="UTF-8"?>
            <event version="2.0" uid="8f6f6794-466d-4404-9b60-557511f9dab1" type="t-x-takp-v"
                time="2023-08-22T01:33:14Z" start="2023-08-22T01:33:14Z" stale="2023-08-22T01:34:14Z"
                how="m-g">
                <point lat="0.0" lon="0.0" hae="0.0" ce="999999" le="999999"/>
                <detail>
                    <TakControl>
                        <TakProtocolSupport version="1"/>
                        <TakServerVersionInfo serverVersion="4.8-RELEASE-45-HEAD"
                            apiVersion="3"/>
                    </TakControl>
                </detail>
            </event>
            """;

        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));

        var @event = await EventReader.ReadFromAsync(stream, CancellationToken.None).ConfigureAwait(false);

        Assert.That(@event, Is.EqualTo(test));
    }
}