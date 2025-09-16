using System.Runtime.CompilerServices;
using FluffyByte.MUDServer.Core.IO.Disk;
using FluffyByte.MUDServer.Core.IO.ErrorTracker;

namespace FluffyByte.MUDServer.Core.Helpers;

public static class Scribe
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static bool DebugMode { get; set; } = true;
    
    private static string LogFilePath { get; set; } = @"./Logs/scribe.log";

    private static readonly IFluffyTextFile LogFile = new FluffyTextFile(LogFilePath);

    private static List<string> _logBuffer = [];
    private const int BufferSize = 1024;
    
    private static readonly Lock LogLocker = new Lock();
    
    public static void Log(string message)
    {
        WriteLine($"[LOG] - {message}");
    }

    public static void Debug(string message)
    {
        if (DebugMode)
            WriteLine($"[DEBUG] - {message}", ConsoleColor.Green);
    }
    
    public static void Error(Exception ex,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null)
    {
        WriteLine($"[ERROR]", ConsoleColor.Red);
        
        FluffyError error = new(ex);
        
        WriteLine(error.ToString(), ConsoleColor.Red);
    }

    public static void Error(string message)
    {
        WriteLine($"[ERROR] - {message}", ConsoleColor.Red);
    }

    public static void Shutdown()
    {
    }
    
    private static void WriteLine(string message, ConsoleColor fgColor = ConsoleColor.White)
    {
        var timestampedMessage = $"{DateTime.UtcNow:yy-MM-dd hh:mm:ss.fff} {message}";
        Console.ResetColor();
        Console.ForegroundColor = fgColor;
        Console.WriteLine(timestampedMessage);
        Console.ResetColor();

        _logBuffer.Add(message);

        lock (LogLocker)
        {
            if (_logBuffer.Count < BufferSize) return;
            
            WriteLogFile();
            _logBuffer.Clear();
        }
    }

    private static void WriteLogFile()
    {
        if (_logBuffer.Count == 0) return;

        LogFile.Lines = new List<string>(_logBuffer);
        
        FluffyTextFileManager.SaveFile(LogFile);
    }
}