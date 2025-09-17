namespace FluffyByte.MUDServer.Game.StandardObjects;

public interface IGameObjectComponent : IDisposable
{
    IGameObject? Owner { get; set; }
}