using System.Runtime.CompilerServices;
using FluffyByte.MUDServer.Core.IO.ErrorTracker;

namespace FluffyByte.MUDServer.Core.IO;

public static class Scribe
{
    public static bool DebugMode { get; set; } = true;
    public static string LogFilePath { get; set; } = "Logs/scribe.log";
    public static string LogFileSettings { get; set; } = "Settings/scribe.settings";

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
    }
}