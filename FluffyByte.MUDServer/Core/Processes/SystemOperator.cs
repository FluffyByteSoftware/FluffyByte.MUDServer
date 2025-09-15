using System.Text;
using FluffyByte.MUDServer.Core.IO;

namespace FluffyByte.MUDServer.Core.Processes;

public sealed class SystemOperator
{
    private static readonly Lazy<SystemOperator> Instance = new(() => new SystemOperator());
    public static SystemOperator Singleton => Instance.Value;
    
    private SystemOperator()  { }

    public List<IFluffyCoreProcess> Processes { get; } = [];
    private List<IFluffyCoreProcess> _started { get; } = [];
    
    public FluffyCoreProcessState State = FluffyCoreProcessState.Stopped;

    private void Initialize()
    {
        if (State == FluffyCoreProcessState.Running)
        {
            Scribe.Debug($"SystemOperator is already running.");
            return;
        }

        Scribe.Log($"Initializing SystemOperator...");
        // Always ensure the sentinel is present.

        _started.Clear();
        Processes.Clear();

        Sentinel sentinel = new();
        Processes.Add(sentinel);
    }
    
    public async Task RequestInitAsync()
    {
        if (Processes.Count == 0)
        {
            Scribe.Log("Processes were not defined. Cannot initialize.");
            return;
        }

        if (State == FluffyCoreProcessState.Running)
        {
            Scribe.Log($"SystemOperator is already running.");
            return;
        }

        Initialize();
        
        foreach (var process in Processes)
        {
            Scribe.Debug($"Attempting to initialize... {process.Name}");
            
            await process.InitializeAsync();
        }
    }
    
    public async Task RequestStartAsync()
    {
        if (Processes.Count == 0)
        {
            Scribe.Log("Processes were not defined. Cannot start.");
            await Task.CompletedTask;
        }
        
        foreach (var process in Processes)
        {
            Scribe.Debug($"Attempting to start... {process.Name}");
            
            await process.RequestStartAsync();
            
            _started.Add(process);
        }
        
        State = FluffyCoreProcessState.Running;
    }

    public async Task RequestStopAsync()
    {
        if (Processes.Count == 0)
        {
            Scribe.Log("Processes were not defined. Cannot stop.");
            await Task.CompletedTask;
        }

        foreach (var process in Processes)
        {
            
            Scribe.Debug($"Attempting to stop... {process.Name}");

            await process.RequestStopAsync();
            
            _started.Remove(process);
        }

        State = FluffyCoreProcessState.Stopped;
    }

    public string RequestProcessStatesAsync()
    {
        StringBuilder sb = new();

        sb.AppendLine("Processes in Processes...");
        
        foreach (var process in Processes)
        {
            sb.AppendLine($"Process: {process.Name} :: State: {process.State}");
        }

        sb.AppendLine("Processes in _started");
        
        foreach (var process in _started)
        {
            sb.AppendLine($"Started Process: {process.Name} :: State: {process.State}");
        }
        
        return sb.ToString();
    }
}