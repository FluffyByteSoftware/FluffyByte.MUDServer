namespace FluffyByte.MUDServer.Core.Processes;

public sealed class Sentinel : FluffyCoreProcessTemplate 
{
    public override string Name => "Sentinel";
    public override FluffyCoreProcessState State { get; protected set; }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default) 
    {

        // Initialization logic here
        await Task.CompletedTask;
    }

    protected override async Task StartAsync() 
    {
        State = FluffyCoreProcessState.Running;
        await Task.CompletedTask;
    }

    protected override async Task StopAsync() 
    {
        State = FluffyCoreProcessState.Stopped;
        await Task.CompletedTask;
    }

}