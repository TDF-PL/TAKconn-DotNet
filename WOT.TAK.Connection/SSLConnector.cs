using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using WOT.TAK.Connection.DTOs;

namespace WOT.TAK.Connection;

internal sealed class SslConnector(
    Url serverUrl,
    Port serverPort,
    string certificatePath,
    string certificatePassword)
    : ITakServerConnector
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private TcpClient? tcpClient;
    private SslStream? sslStream;
    private IEventReceiver? eventReceiver;
    private ResponseListener? responseListener;
    private bool closed;

    public void Connect(IEventReceiver aHandler)
    {
        this.tcpClient = new TcpClient(serverUrl.AsString(), serverPort.AsInt());
        this.sslStream = new SslStream(
            this.tcpClient.GetStream(),
            false,
            this.ValidateServerCertificate,
            null);

        var certificate = new X509Certificate2(certificatePath, certificatePassword);
        var certificates = new X509CertificateCollection { certificate };

        this.sslStream.AuthenticateAsClient(serverUrl.AsString(), certificates, SslProtocols.None, false);

        if (this.sslStream is { IsAuthenticated: true, IsEncrypted: true, IsSigned: true })
        {
            this.eventReceiver = aHandler;
            this.responseListener = ResponseListener.Create(
                this.sslStream, this.eventReceiver, this.cancellationTokenSource);
        }
        else
        {
            throw new AuthenticationException("Failed to authenticate.");
        }
    }

    public void Send(Event anEvent)
    {
        var serializer = new XmlSerializer(typeof(Event));
        serializer.Serialize(this.sslStream!, anEvent);
        this.sslStream!.Flush();
    }

    public void Close()
    {
        if (this.closed)
        {
            return;
        }

        this.cancellationTokenSource.Cancel();
        this.responseListener?.Dispose();
        this.tcpClient?.Close();
        this.sslStream?.Close();
        this.closed = true;
    }

    public void Dispose()
    {
        this.Close();
        this.cancellationTokenSource.Dispose();
    }

    private bool ValidateServerCertificate(
        object sender,
        X509Certificate? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors)
    {
        using var clientCertificate = new X509Certificate2(certificatePath, certificatePassword);

        var clientCertBytes = certificate!.Issuer;
        var serverCertBytes = clientCertificate.Issuer;

        return clientCertBytes.SequenceEqual(serverCertBytes);
    }
}