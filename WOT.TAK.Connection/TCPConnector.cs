using System.IO;
using System.Net.Sockets;

namespace WOT.TAK.Connection;

public class TCPConnector : TAKServerConnector
{
    private readonly int _port;
    private readonly string _responseStoragePath;
    private readonly TcpClient _socket;
    private readonly string _url;
    protected StreamReader _input;
    private Thread _listener;
    protected StreamWriter _output;
    private bool _run;
    private Stream _stream;

    public TCPConnector(string url, string port, string responseStoragePath)
    {
        _url = url;
        _port = int.Parse(port);
        _responseStoragePath = responseStoragePath;
        _socket = new TcpClient();
        _run = true;
    }

    public void SendFile(string fileName)
    {
        try
        {
            using (var sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null) _output.WriteLine(line);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void Connect()
    {
        try
        {
            if (!_socket.Connected)
                _socket.Connect(_url, _port);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        if (_stream == null) _stream = _socket.GetStream();
        _output = new StreamWriter(_stream);
        _input = new StreamReader(_stream);
        _socket.ReceiveTimeout = 1000;
        _listener = new Thread(ResponseListener);
        _listener.IsBackground = true;
        _listener.Start();
    }

    public void Close()
    {
        _run = false;
        Thread.Sleep(200);
        _output.Close();
        _input.Close();
        _socket.Close();
    }

    public void SetStream(Stream stream)
    {
        _stream = stream;
    }

    private void ResponseListener()
    {
        string xml;
        while (_run)
            try
            {
                xml = EventReader.readFrom(_stream);
                using (var sw = File.CreateText(_responseStoragePath + "/" +
                                                DateTimeOffset.Now.ToUnixTimeMilliseconds() + ".cot"))
                {
                    sw.WriteLine(xml);
                }
            }
            catch (SocketException e)
            {
            }
            catch (ObjectDisposedException e)
            {
                Close();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
    }

    public TcpClient GetSocket()
    {
        return _socket;
    }
}