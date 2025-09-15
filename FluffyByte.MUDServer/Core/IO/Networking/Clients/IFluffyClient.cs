using System.Net;
using System.Net.Sockets;

namespace FluffyByte.MUDServer.Core.IO.Networking.Clients;

public interface IFluffyClient
{
    #region Identifiers
    string Name { get; }
    Guid ClientGuid { get; }
    int ClientId { get; }
    #endregion 
    
    #region Networking
    TcpClient TcpClient { get; }
    Socket TcpSocket { get; }
    IPAddress IpAddress { get; }
    string Dns { get; }
    #endregion
    
    #region Date and Time Info
    DateTime ConnectedAt { get; }
    DateTime LastResponse { get; }
    #endregion
    
    #region Messaging
    ValueTask SendMessageAsync(string message);
    ValueTask SendMessageNewlineAsync(string message);
    ValueTask<string> ReceiveMessageAsync();
    #endregion
}