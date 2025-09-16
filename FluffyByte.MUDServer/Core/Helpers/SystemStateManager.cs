namespace FluffyByte.MUDServer.Core.Helpers;

public sealed class SystemStateManager
{
    #region Singleton
    private static readonly Lazy<SystemStateManager> Lazy = new(() => new SystemStateManager());
    public static SystemStateManager Singleton => Lazy.Value;
    private SystemStateManager() { }
    #endregion

    private readonly List<IFluffyCoreProcess> _coreProcesses = [];

    public void AddFluffyCoreProcess(IFluffyCoreProcess coreProcess)
    {
        if(!_coreProcesses.Contains(coreProcess))
            _coreProcesses.Add(coreProcess);
    }

    public void RemoveFluffyCoreProcess(IFluffyCoreProcess coreProcess)
    {
        if (_coreProcesses.Contains(coreProcess))
            _coreProcesses.Remove(coreProcess);
    }
    
    public string ListProcesses()
    {
        if (!_coreProcesses.Any())
            return "No core processes are currently running.";
            
        var processInfo = _coreProcesses.ToFormattedList((process, index) => 
            $"{index + 1}. {process.Name} - State: {process.State}");

        return $"Core Processes: {processInfo}";
    }

}