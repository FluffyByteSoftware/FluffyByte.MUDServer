namespace FluffyByte.MUDServer.Game.StandardObjects.GameObjectComponents;

public sealed class AdjectiveComp(IGameObject owner) : IGameObjectComponent
{
    public List<string> Adjectives { get; private set; } = [];

    public IGameObject? Owner { get; set; } = owner;
    
    public void AddAdjective(string adjective)
    {
        if (Adjectives.Contains(adjective))
            return;

        Adjectives.Add(adjective);
    }

    public void AddAdjectives(string[] adjectives)
    {
        foreach (var adjective in adjectives)
        {
            AddAdjective(adjective);
        }
    }

    public void RemoveAdjective(string adjective)
    {
        if (Adjectives.Contains(adjective))
            Adjectives.Remove(adjective);
    }

    public void RemoveAdjectives(string[] adjectives)
    {
        foreach (var adjective in adjectives)
        {
            RemoveAdjective(adjective);
        }
    }
    
    public void Dispose()
    {
        Adjectives.Clear();
        Owner = null;
    }
}