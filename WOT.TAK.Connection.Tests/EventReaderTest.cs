using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Text;

namespace WOT.TAK.Connection.Tests;

[TestFixture]
public class EventReaderTest
{
    [Test]
    public void testClosedTag()
    {
        String test = "<xml/>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual(test, EventReader.readFrom(stream));
    }

    [Test]
    public void testNoPreamble()
    {
        String test = "<xml></xml>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual(test, EventReader.readFrom(stream));
    }

    [Test]
    public void testPreamble()
    {
        String test = "<?xml?><xml></xml>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual(test, EventReader.readFrom(stream));
    }

    [Test]
    public void testPreambleNewLine()
    {
        String test = "<?xml?>\n<xml></xml>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual(test, EventReader.readFrom(stream));
    }

    [Test]
    public void testTwo()
    {
        String test = "<one></one><two></two>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual("<one></one>", EventReader.readFrom(stream));
        Assert.AreEqual("<two></two>", EventReader.readFrom(stream));
    }

    [Test]
    public void testNewLineTwo()
    {
        String test = "<one></one>\n<two></two>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual("<one></one>", EventReader.readFrom(stream));
        Assert.AreEqual("<two></two>", EventReader.readFrom(stream));
    }

    [Test]
    public void testComplex()
    {
        String test = "\n\n<?x ?>\n\n<one></one>\n\n<?y ?>\n<two></two>\n\n\n";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual("<?x ?>\n\n<one></one>", EventReader.readFrom(stream));
        Assert.AreEqual("<?y ?>\n<two></two>", EventReader.readFrom(stream));
    }

    [Test]
    public void testMix()
    {
        String test = "\n\n<?x ?>\n\n<one></one><two  />\n\n\n";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual("<?x ?>\n\n<one></one>", EventReader.readFrom(stream));
        Assert.AreEqual("<two  />", EventReader.readFrom(stream));
    }

    [Test]
    public void testWhitespace()
    {
        String test = "\n\n\n\n\n";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.IsNull(EventReader.readFrom(stream));
    }

    [Test]
    public void testOneandWhitespace()
    {
        String test = "<one/>\n\n\n\n\n";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual("<one/>", EventReader.readFrom(stream));
        Assert.IsNull(EventReader.readFrom(stream));
        Assert.IsNull(EventReader.readFrom(stream));
    }

    [Test]
    public void testClient()
    {
        String test = "<event version=\"2.0\" uid=\"S-1-5-21-4270384509-2508030920-1516254662-1001\" type=\"a-f-G-U-C-I\" time=\"2023-08-22T01:40:06Z\" start=\"2023-08-22T01:38:12.99Z\" stale=\"2023-08-22T01:44:27.99Z\" how=\"h-g-i-g-o\"><point lat=\"0\" lon=\"0\" hae=\"0\" ce=\"9999999\" le=\"9999999\"/><detail><takv version=\"4.9.0.172\" platform=\"WinTAK-CIV\" os=\"Microsoft Windows 10 Home\" device=\"Apple Inc. MacBookPro16,1\"/><contact callsign=\"MUSIC\" endpoint=\"*:-1:stcp\"/><uid Droid=\"MUSIC\"/><__group name=\"Green\" role=\"Team Member\"/><status battery=\"99\"/><track course=\"0.00000000\" speed=\"0.00000000\"/><_flow-tags_ TAK-Server-c157ed5a69784cc3ab5e2401a5936074=\"2023-08-22T01:40:06Z\"/></detail></event>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual(test, EventReader.readFrom(stream));
    }

    [Test]
    public void testServer()
    {
        String test = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<event version=\"2.0\" uid=\"8f6f6794-466d-4404-9b60-557511f9dab1\" type=\"t-x-takp-v\" time=\"2023-08-22T01:33:14Z\" start=\"2023-08-22T01:33:14Z\" stale=\"2023-08-22T01:34:14Z\" how=\"m-g\"><point lat=\"0.0\" lon=\"0.0\" hae=\"0.0\" ce=\"999999\" le=\"999999\"/><detail><TakControl><TakProtocolSupport version=\"1\"/><TakServerVersionInfo serverVersion=\"4.8-RELEASE-45-HEAD\" apiVersion=\"3\"/></TakControl></detail></event>";
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(test));
        Assert.AreEqual(test, EventReader.readFrom(stream));
    }
}