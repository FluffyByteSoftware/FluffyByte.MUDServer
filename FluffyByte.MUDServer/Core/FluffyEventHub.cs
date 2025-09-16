using FluffyByte.MUDServer.Core.IO;
using FluffyByte.MUDServer.Core.Events;

namespace FluffyByte.MUDServer.Core;

public static class FluffyEventHub
{
    public static Action CreateEvent(string eventName, object? caller = null)
    {
        return () =>
        {
            Scribe.Debug($"{caller ?? "Unknown"} has called event: {eventName}");
        };
    }
}