namespace FluffyByte.MUDServer.Game.StandardObjects;

public interface IGameObjectComponent
{
    GameObject? Owner { get; set; }
}