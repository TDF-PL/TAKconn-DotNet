using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace WOT.TAK.Connection;

internal sealed class Authentication : IDisposable
{
    private readonly X509Certificate2 certificate;
    private readonly X509Certificate2Collection certificateCollection;

    public Authentication(string certPath)
    {
        this.certificate = new X509Certificate2(certPath, "passphrase");
        this.certificateCollection = new X509Certificate2Collection(this.certificate);
    }

    public void AuthenticateX509(string serverName, TcpClient client)
    {
        Console.WriteLine(this.certificate);

        using var sslStream = new SslStream(
                client.GetStream(),
                false,
                ValidateRemoteCertificate!,
                null);

        sslStream.AuthenticateAsClient(serverName, this.certificateCollection, false);
    }

    public void Dispose()
    {
        this.certificate.Dispose();
    }

    private static bool ValidateRemoteCertificate(
        object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return sslPolicyErrors == SslPolicyErrors.None;
    }
}