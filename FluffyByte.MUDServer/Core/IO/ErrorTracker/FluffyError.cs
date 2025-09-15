using System.Text;
using System.Runtime.CompilerServices;

namespace FluffyByte.MUDServer.Core.IO.ErrorTracker;

public class FluffyError(Exception exception,
    [CallerLineNumber] int lineNumber = 0,
    [CallerMemberName] string? memberName = null,
    [CallerFilePath] string? filePath = null) : IFluffyError
{
    public DateTime ErrorTime { get; } = DateTime.UtcNow;
    public string? LineNumber { get; } = lineNumber.ToString();
    public string? MemberName { get; } = memberName;
    public string? FilePath { get; } = filePath;

    public Exception Exception { get; } = exception;
    
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Exception Time (UTC): {ErrorTime:O}");
        sb.AppendLine($"Exception Message: {Exception.Message}");
        sb.AppendLine($"Exception Type: {Exception.GetType().FullName}");
        sb.AppendLine($"Source: {Exception.Source}");
        sb.AppendLine($"Stack Trace: {Exception.StackTrace}");

        if (Exception.InnerException == null) return sb.ToString();
        
        sb.AppendLine("Inner Exception:");
        sb.AppendLine(Exception.InnerException.ToString());
        sb.AppendLine($"Inner Exception Stack Trace: {Exception.InnerException.StackTrace}");

        return sb.ToString();
    }
}