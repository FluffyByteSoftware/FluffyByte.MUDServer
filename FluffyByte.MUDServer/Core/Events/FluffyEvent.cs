namespace FluffyByte.MUDServer.Core.Events;

public class FluffyEvent(string source, string actionName) 
    : FluffyEventArgs(source, actionName)
{
    public FluffyEvent WithData(string key, object value)
    {
        AddData(key, value);
        
        return this;
    }
}