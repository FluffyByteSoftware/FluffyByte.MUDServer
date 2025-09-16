namespace FluffyByte.MUDServer.Core.Events;

public interface IFluffyActionHandler
{
    void HandleAction(string actionName, object[]? args = null);
}