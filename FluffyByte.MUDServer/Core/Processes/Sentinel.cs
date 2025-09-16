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
    private Task? _listeningTask;

    private List<Task> _clientTasks = [];
    
    private static readonly IPAddress HostAddress = IPAddress.Parse("10.0.0.84");
    private static readonly int HostPort = 9998;
    
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        State = FluffyCoreProcessState.Stopped;
        await Task.CompletedTask;
    }

    protected override async Task StartAsync() 
    {
        // Don't set State here - the template handles it
        
        try
        {
            Listener ??= new TcpListener(HostAddress, HostPort);
            Listener.Start();
            
            Scribe.Log($"Sentinel listening on {HostAddress}:{HostPort}");
            
            _listeningTask = ListenForClients();
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
            throw;
        }

        await Task.CompletedTask;
    }

    protected override async Task StopAsync()
    {
        // Don't set State or cancel tokens here - template handles it
        
        Listener?.Stop();

        if (_listeningTask is not null)
        {
            try
            {
                await _listeningTask;
            }
            catch (Exception ex)
            {
                Scribe.Error(ex);
            }
        }

        await Task.CompletedTask;
    }

    private async Task ListenForClients()
    {
        try
        {
            while (!CancellationTokenSource.IsCancellationRequested && Listener is not null)
            {
                var client = await Listener.AcceptTcpClientAsync()
                    .WaitAsync(CancellationTokenSource.Token);

                // Start handling the client and track the task
                var clientTask = HandleClientAsync(client);
                _clientTasks.Add(clientTask);

                // Clean up completed tasks periodically
                _clientTasks.RemoveAll(t => t.IsCompleted);
            }
        }
        catch (OperationCanceledException)
        {
            Scribe.Debug("Listen operation was canceled - stopping client acceptance");
        }
        catch (ObjectDisposedException)
        {
            Scribe.Debug("Listener disposed - stopping client acceptance");
        }
        catch (SocketException)
        {
            Scribe.Debug($"Socket shutdown prematurely.");
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
        }
    
        // Wait for all client tasks to complete during shutdown
        await Task.WhenAll(_clientTasks.Where(t => !t.IsCompleted));
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            Scribe.Debug($"Client connected from {client.Client.RemoteEndPoint}");

            var netClient = new FluffyClient(client);

            await netClient.Messenger.SendMessageAsync("Welcome to FluffyMUD");
            
            await SystemOperator.Singleton.WalmartGreeter.WelcomeNewCustomer(netClient);
        }
        catch (OperationCanceledException)
        {
            Scribe.Debug("Client handling was canceled.");
        }
        catch (IOException)
        {
            Scribe.Debug($"IO was interrupted.");
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
        }
    }
}