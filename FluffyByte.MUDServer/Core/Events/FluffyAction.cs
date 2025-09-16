using FluffyByte.MUDServer.Core.Processes;

namespace FluffyByte.MUDServer.Core.Events;

public class FluffyAction
{
    public string Name { get; }

    public FluffyAction(string name)
    {
        Name = name;
        Oracle.Singleton.RegisterAction(this);
    }

    public void Invoke(params object[]? args)
    {
        Oracle.Singleton.HandleAction(this, args);
    }
}