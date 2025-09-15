namespace FluffyByte.MUDServer.Core.IO.ErrorTracker;

public interface IFluffyError
{
    string ErrorMessage { get; }
    DateTime ErrorTime { get; }
    string? LineNumber { get; }
    string? MemberName { get; }
    string? MemberId { get; }
    
    Exception Exception { get; }
}