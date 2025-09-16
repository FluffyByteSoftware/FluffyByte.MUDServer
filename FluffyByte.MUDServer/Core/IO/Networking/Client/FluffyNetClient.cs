using System.Net.Sockets;
using FluffyByte.MUDServer.Core.IO.Networking.Client.Components;
using FluffyByte.MUDServer.Core.Events;
using FluffyByte.MUDServer.Core.Helpers;

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

    public NetworkDetails Details { get; private set; }
    public Messenger Messenger { get; private set; }

    public bool IsConnected
    {
        get
        {
            try
            {
                bool isConnected = TcpClient.Connected;

                if (isConnected)
                {
                    bool canRead = TcpClient.Client.Poll(0, SelectMode.SelectRead);
                    bool hasData = TcpClient.Client.Available > 0;

                    if (canRead && !hasData)
                        isConnected = false;
                }

                if (!isConnected)
                {
                    _ = Task.Run(async () => await RequestDisconnectAsync());
                }

                return isConnected;
            }
            catch (IOException) { }
            catch (Exception ex)
            {
                Scribe.Error(ex);
                return false;
            }

            return false;
        }
    }

    public FluffyNetClient(TcpClient tcpClient)
    {
        TcpClient = tcpClient;

        Name = tcpClient.Client.RemoteEndPoint?.ToString() ?? "Unknown";
        
        _disconnecting = false;
        _disposing = false;

        Details = new NetworkDetails(tcpClient);
        Messenger = new Messenger(this);
        
        _components.Add(Details);
        _components.Add(Messenger);
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