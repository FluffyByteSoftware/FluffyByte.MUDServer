using FluffyByte.MUDServer.Core.IO;

namespace FluffyByte.MUDServer.Core;

public abstract class FluffyCoreProcessTemplate : IFluffyCoreProcess 
{
    public abstract string Name { get; } 

    public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
    
    public abstract FluffyCoreProcessState State { get; protected set;  }

    public abstract Task InitializeAsync(CancellationToken cancellationToken = default);
    
    public async Task RequestStartAsync(CancellationToken cancellationToken = default) 
    {
        if(State is not FluffyCoreProcessState.Stopped)
            return;

        await StartAsync();

        State = FluffyCoreProcessState.Running;
        Scribe.Debug($"Started Process: {Name} :: {State}");
    }

    public async Task RequestStopAsync(CancellationToken cancellationToken = default) 
    {
        if(State is not FluffyCoreProcessState.Running)
            return;

        await StopAsync();

        await CancellationTokenSource.CancelAsync();

        State = FluffyCoreProcessState.Stopped;
        Scribe.Debug($"Stopped Process: {Name} :: {State}");
    }

    protected abstract Task StartAsync();
    protected abstract Task StopAsync();
}