namespace FluffyByte.MUDServer.Game.StandardObjects.GameObjectComponents;

public sealed class SimpleContainerComp(IGameObject owner) : IGameObjectComponent
{
    public List<IGameObject> Contents { get; private set; } = [];

    public string LongDescription { get; set; } = "Long Description";
    public string ShortDescription { get; set; } = "Short Description";
    public string Name { get; set; } = "object.name";
    public IGameObject? Owner { get; set; } = owner;

    public void InsertItem(IGameObject item)
    {
        if (Contents.Contains(item))
            return;

        Contents.Add(item);
    }

    public IGameObject? WithdrawItem(IGameObject item)
    {
        if (!Contents.Contains(item))
            return null;

        Contents.Remove(item);

        return item;
    }

    public void Dispose()
    {
        Contents.Clear();
        Owner = null;
    }
    
}