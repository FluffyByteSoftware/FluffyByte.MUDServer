using System.Net;
using System.Net.Sockets;

namespace FluffyByte.MUDServer.Core.IO.Networking.Client.Components;

public sealed class NetworkDetails : IFluffyClientComponent
{
    public string Name => "Network Details";
    public CancellationTokenSource Cts { get; set; }

    public string IpString { get; private set; } = "0.0.0.0";
    public IPAddress IpAddress { get; private set; } = IPAddress.None;
    public IPEndPoint? IpEndPoint { get; private set; }
    public string DnsName { get; private set; } = "unknown.host";

    public NetworkDetails(TcpClient parent)
    {
        IpEndPoint = parent.Client.RemoteEndPoint as IPEndPoint 
                     ?? throw new NullReferenceException(
                         $"{nameof(parent.Client)}.{nameof(parent.Client.RemoteEndPoint)} is null.");
        
        ResolveIpDetails(parent);

        Cts = new CancellationTokenSource();
    }

    private void ResolveIpDetails(TcpClient parent)
    {
        try
        {
            IpEndPoint = parent.Client.RemoteEndPoint as IPEndPoint;
            
            if (IpEndPoint != null)
            {
                IpAddress = IpEndPoint.Address;
                
                IpString = IpAddress.ToString();
                try
                {
                    var hostEntry = Dns.GetHostEntry(IpAddress);
                    DnsName = hostEntry.HostName;
                }
                catch (SocketException)
                {
                    DnsName = "Unknown";
                }
            }
            else
            {
                IpAddress = IPAddress.None;
                IpString = "0.0.0.1";
                DnsName = "unknown.host.remote";
            }
        }
        catch (Exception ex)
        {
            Scribe.Error(ex);
            IpAddress = IPAddress.None;
            IpString = "0.0.0.1";
            DnsName = "unknown.host.remote";
        }
        
    }
}