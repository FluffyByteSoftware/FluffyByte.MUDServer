using FluffyByte.MUDServer.Core.Helpers;

namespace FluffyByte.MUDServer.Core.IO;

public abstract class Program
{
    public static async Task Main(string[] args)
    {
        Scribe.Log("Initializing System Operator...");
        await SystemOperator.Singleton.RequestInitAsync();

        Scribe.Log("Starting System Operator...");
        
        await SystemOperator.Singleton.RequestStartAsync();
        
        Scribe.Log(SystemOperator.Singleton.RequestProcessStates());

        Scribe.Log("Press any key to shutdown.");
        Console.ReadLine();
        
        await SystemOperator.Singleton.RequestStopAsync();
        
        //Last operation
        Scribe.Shutdown();
    }
}