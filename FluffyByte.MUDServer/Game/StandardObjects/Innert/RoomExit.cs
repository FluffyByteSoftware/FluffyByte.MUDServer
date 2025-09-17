using System.Runtime.CompilerServices;

namespace FluffyByte.MUDServer.Game.StandardObjects.Innert;

public class RoomExit(string exitDirection, string fileToRoom) : GameObject(exitDirection)
{
    public string ExitName { get; set; } = exitDirection;
    public string ExitRoomPath { get; set; } = fileToRoom;
}