using System.Runtime.CompilerServices;
using System.Text;
using FluffyByte.MUDServer.Core.IO.Disk;
using FluffyByte.MUDServer.Core.IO.ErrorTracker;

namespace FluffyByte.MUDServer.Core.IO;

public static class Scribe
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static bool DebugMode { get; set; } = true;
    
    private static string LogFilePath { get; set; } = "Logs/scribe.log";

    private static readonly IFluffyTextFile LogFile = new FluffyTextFile(LogFilePath);
    
    public static void Log(string message)
    {
        WriteLine($"[LOG - {DateTime.UtcNow} ] - {message}");
    }

    public static void Debug(string message)
    {
        if (DebugMode)
            WriteLine($"[DEBUG - {DateTime.UtcNow} ] - {message}", ConsoleColor.Green);
    }
    
    public static void Error(Exception ex,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null)
    {
        WriteLine($"[ ERROR ENCOUNTERED ]", ConsoleColor.Red);
        
        FluffyError error = new(ex);
        
        WriteLine(error.ToString(), ConsoleColor.Red);
    }

    public static void Error(string message)
    {
        WriteLine($"[ ERROR ENCOUNTERED ] - {message}", ConsoleColor.Red);
    }

    private static void WriteLine(string message, ConsoleColor fgColor = ConsoleColor.White)
    {
        Console.ResetColor();
        Console.ForegroundColor = fgColor;
        Console.WriteLine(message);
        Console.ResetColor();

        bool result = FluffyTextFileManager.SaveFile(LogFile, encoding: Encoding.UTF8, createDirectory: true);

        if (result && DebugMode)
            Console.WriteLine("Updated logfile.");
        else
            Console.WriteLine("Failed to save logfile!");
    }
}