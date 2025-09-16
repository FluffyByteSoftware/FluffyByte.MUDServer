using System.Runtime.CompilerServices;
using FluffyByte.MUDServer.Core.Helpers;

namespace FluffyByte.MUDServer.Core.Events;

public class FluffyAction
{

    public string Name { get; private set; }

    public FluffyAction([CallerMemberName]string name = "")
    {
        Name = name;
        Oracle.Singleton.RegisterAction(this);
    }
    
    public static FluffyAction Create([CallerMemberName] string name = "")
    {
        return new FluffyAction(name);
    }

    public void Invoke(params object[]? args)
    {
        Oracle.Singleton.HandleAction(this, args);
    }
}