using FluffyByte.MUDServer.Core.Processes;

namespace FluffyByte.MUDServer.Core.IO;

public abstract class Program
{
    public static async Task Main(string[] args)
    {
        SystemOperator.Singleton.Initialize();

        Console.WriteLine("Loaded...");

        Scribe.Log($"SystemOperator is: {SystemOperator.Singleton.State}");

        Console.ReadLine();

        await SystemOperator.Singleton.RequestStopAsync();
    }
}