namespace FluffyByte.MUDServer.Core.IO.ErrorTracker;

public interface IFluffyError
{
    DateTime ErrorTime { get; }
    string? LineNumber { get; }
    string? MemberName { get; }
    string? FilePath { get; }
    Exception Exception { get; }
}