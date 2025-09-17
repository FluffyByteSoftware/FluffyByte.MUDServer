using System.ComponentModel;
using FluffyByte.MUDServer.Game.StandardObjects;

namespace FluffyByte.MUDServer.Game.StandardObjects.Innert;

public class GameRoom(string name) : GameObject(name)
{
    public string Description { get; set; } = "An empty room.";
}