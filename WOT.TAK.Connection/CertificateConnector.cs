using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WOT.TAK.Connection;

public class CertificateConnector : TAKServerConnector
{
    protected SslStream _authConnection;
    protected string _certPass;
    protected string _certPath;
    protected string _cotResponsesDirPath;
    protected string _serverPort;
    protected string _serverUrl;

    public CertificateConnector(
        string serverUrl,
        string serverPort,
        string cotResponsesDirPath,
        string certPath,
        string certPass)
    {
        _serverUrl = serverUrl;
        _serverPort = serverPort;
        _cotResponsesDirPath = cotResponsesDirPath;
        _certPath = certPath;
        _certPass = certPass;
    }

    public void Close()
    {
        _authConnection.Close();
    }

    public void Connect()
    {
        var connector = new TCPConnector(_serverUrl, _serverPort, _cotResponsesDirPath);
        connector.GetSocket().Connect(_serverUrl, int.Parse(_serverPort));

        var fileStream = File.OpenRead(_certPath);
        var buffer = new byte[fileStream.Length];
        var bytesRead = fileStream.Read(buffer, 0, (int)fileStream.Length);

        var clientCertificate = new X509Certificate2(buffer, _certPass);
        var certificateCollection = new X509Certificate2Collection(clientCertificate);

        _authConnection = new SslStream(
            connector.GetSocket().GetStream(),
            false,
            ValidateServerCertificate,
            SelectUserCertificate);

        // Funkcja powodująca problemy
        _authConnection.AuthenticateAsClient(_serverUrl, certificateCollection, SslProtocols.Tls12, false);
        connector.SetStream(_authConnection);
        connector.Connect();
    }

    public void SendFile(string path)
    {
        try
        {
            using (var sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null) _authConnection.Write(Encoding.ASCII.GetBytes(line));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private X509Certificate2 SelectUserCertificate(
        object sender,
        string targetHost,
        X509CertificateCollection collection,
        X509Certificate remoteCert,
        string[] acceptableIssuers)
    {
        var fileStream = File.OpenRead(_certPath);
        var buffer = new byte[fileStream.Length];
        var bytesRead = fileStream.Read(buffer, 0, (int)fileStream.Length);

        var clientCertificate = new X509Certificate2(buffer, _certPass);
        var certificateCollection = new X509Certificate2Collection(clientCertificate);
        return clientCertificate;
    }

    private bool ValidateServerCertificate(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors)
    {
        var fileStream = File.OpenRead(_certPath);
        var buffer = new byte[fileStream.Length];
        var bytesRead = fileStream.Read(buffer, 0, (int)fileStream.Length);

        var clientCertificate = new X509Certificate2(buffer, _certPass);
        var certificateCollection = new X509Certificate2Collection(clientCertificate);
        try
        {
            var clientCertBytes = certificate.Issuer;
            var serverCertBytes = clientCertificate.Issuer;
            if (clientCertBytes.Length != serverCertBytes.Length)
                throw new Exception("Client/server certificates do not match.");
            for (var i = 0; i < clientCertBytes.Length; i++)
                if (clientCertBytes[i] != serverCertBytes[i])
                    throw new Exception("Client/server certificates do not match.");
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
}