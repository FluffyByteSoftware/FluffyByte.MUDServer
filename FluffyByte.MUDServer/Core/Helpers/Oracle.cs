using FluffyByte.MUDServer.Core.IO;
using FluffyByte.MUDServer.Core.Events;

namespace FluffyByte.MUDServer.Core.Helpers;

public class Oracle
{
    private static readonly Lazy<Oracle> Instance = new(() => new());
    public static Oracle Singleton => Instance.Value;
    
    private readonly Dictionary<string, List<IFluffyActionHandler>> _handlers = new();
    
    private Oracle() { }
    
    public void RegisterAction(FluffyAction action)
    {
        if (!_handlers.ContainsKey(action.Name))
        {
            _handlers[action.Name] = new List<IFluffyActionHandler>();
        }
    }
    
    public void HandleAction(FluffyAction action, object[]? args = null)
    {
        if (args == null || args.Length == 0)
        {
            Scribe.Debug($"Action {action.Name} was activated.");
        }
        else
        {
            Scribe.Debug($"Action {action.Name} was activated with {args.Length} parameters.");
        }
        
        // Notify registered handlers
        if (_handlers.TryGetValue(action.Name, out var handlers))
        {
            foreach (var handler in handlers)
            {
                try
                {
                    handler.HandleAction(action.Name, args);
                }
                catch (Exception ex)
                {
                    Scribe.Error($"Error handling action {action.Name}: {ex.Message}");
                }
            }
        }
    }
    
    public void Subscribe(string actionName, IFluffyActionHandler handler)
    {
        if (_handlers.TryGetValue(actionName, out var handlers))
        {
            handlers.Add(handler);
        }
    }
    
    public void Unsubscribe(string actionName, IFluffyActionHandler handler)
    {
        if (_handlers.TryGetValue(actionName, out var handlers))
        {
            handlers.Remove(handler);
        }
    }
}