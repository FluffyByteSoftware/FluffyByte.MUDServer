using System.Net.Sockets;
using System.Net;
using FluffyByte.MUDServer.Core.Helpers;
using FluffyByte.MUDServer.Core.IO.Networking.Client;

namespace FluffyByte.MUDServer.Core.Processes;

public sealed class Sentinel : FluffyCoreProcessTemplate 
{
    public override string Name => "Sentinel";
    public override FluffyCoreProcessState State { get; protected set; }

    private TcpListener? Listener { get; set; }
    
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        
        await Task.CompletedTask;
    }

    protected override async Task StartAsync() 
    {
        State = FluffyCoreProcessState.Running;

        Listener ??= new TcpListener(localaddr: IPAddress.Parse("10.0.0.84"), 
                port: 9998);
        
        Listener?.Start();

        _ = ListenForClients();

        await Task.CompletedTask;
    }

    protected override async Task StopAsync() 
    {
        State = FluffyCoreProcessState.Stopped;
        await Task.CompletedTask;
    }

    private async Task ListenForClients()
    {
        try
        {
            while (!CancellationTokenSource.IsCancellationRequested
                   && Listener is not null)
            {
                var client = await Listener.AcceptTcpClientAsync();
                var netClient = new FluffyNetClient(client);

                await netClient.Messenger.SendMessageAsync("Welcome to FluffyByte");

                var response = await netClient.Messenger.ReadMessageAsync();
                
                await netClient.Messenger.SendMessageAsync(response);
                
                await netClient.RequestDisconnectAsync();
            }
        }
        catch (IOException)
        {
            Scribe.Debug($"IOException in ListenForClients. " +
                         $"Likely due to listener being stopped.");
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
        }
    }
}