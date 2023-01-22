using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WOT.TAK.Connection;

namespace WOT.TAK.Client;

public class Program
{
    private static void Main(string[] args)
    {
        using var host = CreateHostBuilder().Build();

        // Invoke Worker
        using var serviceScope = host.Services.CreateScope();
        var provider = serviceScope.ServiceProvider;
        var client = provider.GetRequiredService<Client>();
        client.DoWork();

        host.Run();
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                configuration.AddJsonFile("appsettings.json", false, true);
            })
            .ConfigureServices((builder, services) =>
            {
                services.AddSingleton<Client>()
                    .AddSingleton<ConnectorFactory>()
                    .Configure<ConnectorSettings>(builder.Configuration);
            });
    }
}

// Worker.cs
internal class Client
{
    private readonly ConnectorFactory _connectorFactory;

    public Client(ConnectorFactory connectorFactory)
    {
        _connectorFactory = connectorFactory;
    }

    public void DoWork()
    {
        var connector = new CertificateConnector("212.160.99.185", "8089", @"cot\responses", @"cot\cert\user.p12",
            @"atakatak");
        connector.Connect();
        connector.SendFile("cot/messages/msg1.cot");
        connector.SendFile("cot/messages/msg2.cot");
        connector.SendFile("cot/messages/msg3.cot");
        Thread.Sleep(2000);
        connector.Close();
        Console.WriteLine("Sent");
    }
}