using System.Text;
using FluffyByte.MUDServer.Core.Events;
using FluffyByte.MUDServer.Core.Helpers;
using FluffyByte.MUDServer.Core.Processes;

namespace FluffyByte.MUDServer.Core;

public sealed class SystemOperator
{
    private static readonly Lazy<SystemOperator> Instance = new(() => new SystemOperator());
    public static SystemOperator Singleton => Instance.Value;
    
    private SystemOperator()  { }

    private readonly List<IFluffyCoreProcess> _processes = [];
    private readonly List<IFluffyCoreProcess> _started = [];

    public Sentinel Sentinel { get; private set; } = new();
    public WalmartGreeter WalmartGreeter { get; private set; } = new();
    
    public FluffyCoreProcessState State { get; private set; } = FluffyCoreProcessState.Stopped;

    private readonly FluffyAction _onInitRequested = new();
    private readonly FluffyAction _onInitialized = new();
    private readonly FluffyAction _onShutdownRequested = new();
    private readonly FluffyAction _onShutdown = new();
    private readonly FluffyAction _onStartRequested = new();
    private readonly FluffyAction _onStarted = new();
    private readonly FluffyAction _onChildStartRequested = new();
    private readonly FluffyAction _onChildStarted = new();
    
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
        _processes.Clear();

        _onInitialized.Invoke();

        Sentinel = new Sentinel();
        WalmartGreeter = new WalmartGreeter();
        
        _processes.Add(Sentinel);
        _processes.Add(WalmartGreeter);

        foreach (var process in _processes)
        {
            process.InitializeAsync();
        }
    }
    
    public async Task RequestInitAsync()
    {
        if (State == FluffyCoreProcessState.Running)
        {
            Scribe.Log($"SystemOperator is already running.");
            return;
        }

        _onInitRequested.Invoke();

        Initialize();

        _onInitialized.Invoke();
            
        State = FluffyCoreProcessState.Stopped;
        
        await Task.CompletedTask;
    }
    
    public async Task RequestStartAsync()
    {
        if (_processes.Count == 0)
        {
            Scribe.Log("Processes were not defined. Cannot start.");
            await Task.CompletedTask;
        }

        _onStartRequested.Invoke();
        
        foreach (var process in _processes)
        {
            Scribe.Debug($"Attempting to start... {process.Name}");
            
            _onChildStartRequested.Invoke();
            
            await process.RequestStartAsync();
            
            _started.Add(process);
            
            _onChildStarted.Invoke();
        }
        
        this.State = FluffyCoreProcessState.Running;
        _onStarted.Invoke();
    }

    public async Task RequestStopAsync()
    {
        if (_processes.Count == 0 || State is FluffyCoreProcessState.Stopped)
        {
            Scribe.Log("Processes were not started. Cannot stop.");
            await Task.CompletedTask;
        }

        foreach (var process in _processes)
        {
            _onShutdownRequested.Invoke();
            
            Scribe.Debug($"Attempting to stop... {process.Name}");

            await process.RequestStopAsync();
            
            _started.Remove(process);
            
            _onShutdown.Invoke();
        }

        State = FluffyCoreProcessState.Stopped;
    }

    public string RequestProcessStates()
    {
        StringBuilder sb = new();

        sb.AppendLine("Processes in Processes...");
        
        foreach (var process in _processes)
        {
            sb.AppendLine($"Process: {process.Name} :: State: {process.State}");
        }

        sb.AppendLine("Processes in _started");
        
        foreach (var process in _started)
        {
            sb.AppendLine($"_started Process: {process.Name} :: State: {process.State}");
        }
        
        return sb.ToString();
    }
}