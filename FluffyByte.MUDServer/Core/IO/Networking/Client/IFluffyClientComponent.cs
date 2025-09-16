namespace FluffyByte.MUDServer.Core.IO.Networking.Client;

public interface IFluffyClientComponent
{
    string Name { get; }
    CancellationTokenSource Cts { get; set; }
}