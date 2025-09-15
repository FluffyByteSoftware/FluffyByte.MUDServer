namespace FluffyByte.MUDServer.Core;

public abstract class FluffyCoreProcessTemplate : IFluffyCoreProcess 
{
    public abstract string Name { get; } 

    public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
    
    public abstract FluffyCoreProcessState State { get; set; }

    public abstract Task InitializeAsync(CancellationToken cancellationToken = default);
    
    public async Task RequestStartAsync(CancellationToken cancellationToken = default) 
    {
        if(State is not FluffyCoreProcessState.Stopped)
            return;

        await StartAsync();

        State = FluffyCoreProcessState.Running;
    }

    public async Task RequestStopAsync(CancellationToken cancellationToken = default) 
    {
        if(State is not FluffyCoreProcessState.Running)
            return;

        await StopAsync();

        CancellationTokenSource.Cancel();

        State = FluffyCoreProcessState.Stopped;
    }

    public abstract Task StartAsync();
    public abstract Task StopAsync();
}