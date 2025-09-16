using FluffyByte.MUDServer.Core.Helpers;
using FluffyByte.MUDServer.Game.StandardObjects;

namespace FluffyByte.MUDServer.Core.IO;

public abstract class Program
{
    public static async Task Main(string[] args)
    {
        Scribe.Log("Initializing FluffyByte MUD Server Core...");
        
        await SystemOperator.Singleton.RequestInitAsync();
        Scribe.Log("Booting startup processes...");
        await SystemOperator.Singleton.RequestStartAsync();
        
        Scribe.Log(SystemOperator.Singleton.RequestProcessStates());

        GameObject sword = new("sword");
        
        
        Scribe.Log("Press any key to shutdown.");
        Console.ReadLine();
        
        await SystemOperator.Singleton.RequestStopAsync();
        
        //Last operation
        Scribe.Shutdown();
    }
}