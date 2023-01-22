using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WOT.TAK.Connection;

public class SSLConnector : TAKServerConnector
{
    private readonly string _responseStoragePath;
    private readonly int _serverPort;
    private readonly string _serverUrl;
    private TcpClient _client;
    private SslStream _sslStream;

    internal SSLConnector(string serverUrl, string serverPort, string responseStoragePath)
    {
        _serverUrl = serverUrl;
        _serverPort = int.Parse(serverPort);
        _responseStoragePath = responseStoragePath;
    }

    public void Close()
    {
        _client.Close();
    }

    public void Connect()
    {
        var _client = new TcpClient(_serverUrl, _serverPort);
        Console.WriteLine("Client connected.");
        _sslStream = new SslStream(
            _client.GetStream(),
            false,
            ValidateServerCertificate,
            null
        );
        try
        {
            _sslStream.AuthenticateAsClient(_serverUrl);
        }
        catch (AuthenticationException e)
        {
            Console.WriteLine("Exception: {0}", e.Message);
            if (e.InnerException != null) Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
            Console.WriteLine("Authentication failed - closing the connection.");
            _client.Close();
        }
    }

    public void SendFile(string path)
    {
        var msg = new StringBuilder();
        using (StreamReader file = new(path))
        {
            string line;
            while ((line = file.ReadLine()) != null) msg.Append(line);
            file.Close();
        }

        var data = Encoding.ASCII.GetBytes(msg.ToString());
        _sslStream.Write(data);
        _sslStream.Flush();
        var serverMessage = ReadMessage(_sslStream);
        using (var sw = File.CreateText(_responseStoragePath + "/" + DateTimeOffset.Now.ToUnixTimeMilliseconds() +
                                        ".cot"))
        {
            sw.Write(serverMessage);
        }
    }

    public static bool ValidateServerCertificate(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

        return false;
    }

    private static string ReadMessage(SslStream sslStream)
    {
        var buffer = new byte[2048];
        var messageData = new StringBuilder();
        var bytes = -1;
        do
        {
            bytes = sslStream.Read(buffer, 0, buffer.Length);

            var decoder = Encoding.UTF8.GetDecoder();
            var chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
            decoder.GetChars(buffer, 0, bytes, chars, 0);
            messageData.Append(chars);
            if (messageData.ToString().IndexOf("<EOF>") != -1) break;
        } while (bytes != 0);

        return messageData.ToString();
    }
}