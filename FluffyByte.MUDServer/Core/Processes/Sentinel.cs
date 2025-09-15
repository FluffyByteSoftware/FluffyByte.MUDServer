using System.Net.Sockets;
using System.Net;
using FluffyByte.MUDServer.Core.IO;

namespace FluffyByte.MUDServer.Core.Processes;

public sealed class Sentinel : FluffyCoreProcessTemplate 
{
    public override string Name => "Sentinel";
    public override FluffyCoreProcessState State { get; protected set; }

    public TcpListener? Listener { get; private set; }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        
        Listener = new TcpListener(localaddr: IPAddress.Parse("10.0.0.84"), 
            port: 9998);
        
        // Initialization logic here
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
        while (!CancellationTokenSource.IsCancellationRequested 
               && Listener is not null)
        {
            var socket = await Listener.AcceptSocketAsync();
            
            Scribe.Log($"Client connected: {socket.RemoteEndPoint}");
        }

        await Task.CompletedTask;
    }
}