using System.Net.Sockets;
using FluffyByte.MUDServer.Core.Events;
using FluffyByte.MUDServer.Core.IO.Networking.Client.Components;

namespace FluffyByte.MUDServer.Core.IO.Networking.Client;

public interface IFluffyClient : IAsyncDisposable
{
    string Name { get; }
    TcpClient TcpClient { get; }
    
    CancellationTokenSource CancelMe { get; }

    bool IsConnected { get; }

    Task RequestDisconnectAsync();
    
    FluffyAction OnConnected { get; }
    FluffyAction OnDisconnectRequested { get; }
    FluffyAction OnDisconnected { get; }

    Messenger Messenger { get; }
    NetworkDetails Details { get; }
}