using FluffyByte.MUDServer.Core.IO;

namespace FluffyByte.MUDServer.Core.Events;

public class FluffyAction(string name)
{
    private readonly List<Action<FluffyEvent>> _actions = new();
    private readonly Lock _lock = new Lock();
    
    public string Name { get; } = name;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    public int SubscriberCount 
    { 
        get 
        { 
            lock (_lock) 
            { 
                return _actions.Count; 
            } 
        } 
    }
    
    public void Subscribe(Action<FluffyEvent> action)
    {
        lock (_lock)
        {
            _actions.Add(action);
        }
    }
    
    public void Unsubscribe(Action<FluffyEvent> action)
    {
        lock (_lock)
        {
            _actions.Remove(action);
        }
    }
    
    public void Invoke(FluffyEvent eventArgs)
    {
        List<Action<FluffyEvent>> actionsToInvoke;
        lock (_lock)
        {
            actionsToInvoke = new List<Action<FluffyEvent>>(_actions);
        }
        
        foreach (var action in actionsToInvoke)
        {
            try
            {
                action(eventArgs);
            }
            catch (Exception ex)
            {
                Scribe.Error(ex);
            }
        }
    }
}