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

    public FluffyAction OnConnected { get; } = new("OnConnected");
    public FluffyAction OnDisconnected { get; } = new("OnDisconnected");
    public FluffyAction OnDisconnectRequested { get; } = new("OnDisconnectRequested");

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

    public MessengerTool MessengerTool { get; init; }

    public FluffyNetClient(TcpClient tcpClient)
    {
        TcpClient = tcpClient;

        Name = tcpClient.Client.RemoteEndPoint?.ToString() ?? "Unknown";
        
        MessengerTool = new MessengerTool(this);

        _components.Add(MessengerTool);
        
        _disconnecting = false;
        _disposing = false;
    }

    public async Task SendMessage(string message, bool newline = true)
    {
        await MessengerTool.SendMessageAsync(message, newline);
    }

    public async Task<string> ReceiveMessage()
    {
        var response = await MessengerTool.ReadMessageAsync();

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