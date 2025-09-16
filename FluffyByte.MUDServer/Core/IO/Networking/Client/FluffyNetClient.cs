using System.Net.Sockets;
using FluffyByte.MUDServer.Core.IO.Networking.Client.Components;
using FluffyByte.MUDServer.Core.Events;

namespace FluffyByte.MUDServer.Core.IO.Networking.Client;

public sealed class FluffyNetClient : IFluffyClient
{
    public string Name { get; private set; }
    public TcpClient TcpClient { get; private set; }

    private bool _disconnecting;
    private bool _disposing;
    
    private List<IFluffyClientComponent> _components = [];

    public CancellationTokenSource CancelMe { get; private set; } = new();

    public FluffyAction OnConnected { get; set; }
    public FluffyAction OnDisconnected { get; set; }
    public FluffyAction OnDisconnectRequested { get; set; }

    public bool IsConnected
    {
        get
        {
            try
            {
                
                return true;
            }
            catch (Exception ex)
            {
                Scribe.Error(ex);
                return false;
            }
        }
    }

    public FluffyClientMessenger Messenger { get; private set; }

    public FluffyNetClient(TcpClient tcpClient)
    {
        TcpClient = tcpClient;

        Name = tcpClient.Client.RemoteEndPoint?.ToString() ?? "Unknown";
        
        Messenger = new FluffyClientMessenger(this);

        _components.Add(Messenger);
        
        _disconnecting = false;
        _disposing = false;

        OnConnected = FluffyEventHub.CreateEvent("Client Connected");

    }

    public async Task SendMessage(string message, bool newline = true)
    {
        await Messenger.SendMessageAsync(message, newline);
    }

    public async Task<string> ReceiveMessage()
    {
        var response = await Messenger.ReadMessageAsync();

        return response;
    }

    public async Task RequestDisconnectAsync()
    {
        if (_disconnecting) return;

        _disconnecting = true;
        
        Scribe.Debug($"Requested a disconnect for client: {Name}");

        try
        {
            foreach (var c in _components)
            {
                await c.Cts.CancelAsync();
            }
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
        }
        finally
        {
            
            TcpClient.Close();

            await RequestDisconnectAsync();
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_disposing) return;

        _disposing = true;
        
        try
        {
            if(!_disconnecting)
                await RequestDisconnectAsync();
            
            TcpClient.Close();
            TcpClient.Dispose();
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
        }
    }
    
}