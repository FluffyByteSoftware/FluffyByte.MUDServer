using FluffyByte.MUDServer.Core.Processes;

namespace FluffyByte.MUDServer.Core.IO;

public abstract class Program
{
    public static async Task Main(string[] args)
    {
        await SystemOperator.Singleton.RequestInitAsync();

        Scribe.Log($"SystemOperator is: {SystemOperator.Singleton.State}");

        await SystemOperator.Singleton.RequestStartAsync();
        Scribe.Debug($"Bootup complete.");
        
        Console.ReadLine();

        await SystemOperator.Singleton.RequestStopAsync();
    }
}