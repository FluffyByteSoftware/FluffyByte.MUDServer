namespace FluffyByte.MUDServer.Game.StandardObjects;

public sealed class GameObject(string name) : IGameObject
{
    public Guid ObjectGuid { get; private set; } = Guid.NewGuid(); 
    
    public string GameName { get; set; } = name;
    
    private readonly Dictionary<Type, IGameObjectComponent> _components = [];

    public T AddComponent<T>(T component) where T : class, IGameObjectComponent
    {
        var componentType = typeof(T);

        if (_components.TryGetValue(componentType, out var componentOut))
        {
            componentOut.Owner = null;
        }

        component.Owner = this;
        _components[componentType] = component;

        return component;
    }

    public T? GetComponent<T>() where T : class, IGameObjectComponent
    {
        var componentType = typeof(T);
        
        return _components.TryGetValue(componentType, out var component) ? component as T : null;
    }

    public bool HasComponent<T>() where T : class, IGameObjectComponent
    {
        return _components.ContainsKey(typeof(T));
    }
        
    public IEnumerable<IGameObjectComponent> GetAllComponents()
    {
        return _components.Values.ToList();
    }

    public void OnAttached()
    {
        
    }

    public void OnDetached()
    {
        
    }

}