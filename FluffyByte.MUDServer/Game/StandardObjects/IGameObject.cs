namespace FluffyByte.MUDServer.Game.StandardObjects;

public interface IGameObject : IDisposable
{
    string GameName { get; set; }
    Guid ObjectGuid { get; }

    T AddComponent<T>(T component) where T : class, IGameObjectComponent;
    T? GetComponent<T>() where T : class, IGameObjectComponent;
    bool HasComponent<T>() where T : class, IGameObjectComponent;
    
    IEnumerable<IGameObjectComponent> GetAllComponents();
}
