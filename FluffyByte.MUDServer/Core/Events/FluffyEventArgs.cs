namespace FluffyByte.MUDServer.Core.Events;

public abstract class FluffyEventArgs : EventArgs
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string EventId { get; } = Guid.NewGuid().ToString();
    public string Source { get; set; }
    public string ActionName { get; set; }
    public bool Handled { get; set; } = false;
    public Dictionary<string, object> Data { get; } = new();
    
    protected FluffyEventArgs(string source, string actionName)
    {
        Source = source;
        ActionName = actionName;
    }
    
    public void AddData(string key, object value)
    {
        Data[key] = value;
    }
    
    public bool TryGetData<T>(string key, out T? value)
    {
        if (Data.TryGetValue(key, out var obj) && obj is T data)
        {
            value = data;
            return true;
        }
        
        value = default(T);
        return false;
    }
}