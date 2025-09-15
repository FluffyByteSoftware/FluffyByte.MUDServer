using FluffyByte.MUDServer.Core;

namespace FluffyByte.MUDServer.Core.Processes;

public sealed class Sentinel : FluffyCoreProcessTemplate 
{
    public override string Name => "Sentinel";
    public override FluffyCoreProcessState State { get; set; }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default) 
    {

        // Initialization logic here
        await Task.CompletedTask;
    }

    public override async Task StartAsync() 
    {
        State = FluffyCoreProcessState.Running;
        await Task.CompletedTask;
    }

    public override async Task StopAsync() 
    {
        State = FluffyCoreProcessState.Stopped;
        await Task.CompletedTask;
    }

}