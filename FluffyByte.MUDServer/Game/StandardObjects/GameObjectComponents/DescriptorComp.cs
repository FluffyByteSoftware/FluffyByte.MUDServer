namespace FluffyByte.MUDServer.Game.StandardObjects.GameObjectComponents;

public sealed class DescriptorComp(GameObject owner) : IGameObjectComponent
{
    public string LongDescription { get; set; } = "Long Description";
    public string ShortDescription { get; set; } = "Short Description";
    public string Name { get; set; } = "object.name";
    public IGameObject? Owner { get; set; } = owner;

    public void Dispose()
    {
        Owner = null;
    }
}