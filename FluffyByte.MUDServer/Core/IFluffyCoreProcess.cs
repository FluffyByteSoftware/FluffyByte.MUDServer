namespace FluffyByte.MUDServer.Core;

public interface IFluffyCoreProcess 
{
    string Name { get; }

    CancellationTokenSource CancellationTokenSource { get; set; }

    FluffyCoreProcessState State { get; }

    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task RequestStartAsync(CancellationToken cancellationToken = default);
    Task RequestStopAsync(CancellationToken cancellationToken = default);
}