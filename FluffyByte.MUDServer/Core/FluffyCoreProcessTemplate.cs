using FluffyByte.MUDServer.Core.Events;

namespace FluffyByte.MUDServer.Core;

public abstract class FluffyCoreProcessTemplate : IFluffyCoreProcess 
{
    public abstract string Name { get; } 

    public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
    
    public abstract FluffyCoreProcessState State { get; protected set;  }

    public abstract Task InitializeAsync(CancellationToken cancellationToken = default);

    private readonly FluffyAction _requestStart = new FluffyAction();
    private readonly FluffyAction _started = new FluffyAction();
    private readonly FluffyAction _requestStop = new FluffyAction();
    private readonly FluffyAction _stopped = new FluffyAction();
    
    public async Task RequestStartAsync(CancellationToken cancellationToken = default) 
    {
        if(State is not FluffyCoreProcessState.Stopped)
            return;

        _requestStart.Invoke();
        
        await StartAsync();

        State = FluffyCoreProcessState.Running;
        _started.Invoke();
    }

    public async Task RequestStopAsync(CancellationToken cancellationToken = default) 
    {
        if(State is not FluffyCoreProcessState.Running)
            return;

        _requestStop.Invoke();
        await StopAsync();

        await CancellationTokenSource.CancelAsync();

        State = FluffyCoreProcessState.Stopped;

        _stopped.Invoke();
    }

    protected abstract Task StartAsync();
    protected abstract Task StopAsync();
}