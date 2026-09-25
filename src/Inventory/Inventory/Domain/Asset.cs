namespace Inventory.Domain;

public sealed class Asset
{
    private Asset()
    {
    }

    public Asset(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public ICollection<DeskAsset> Desks { get; } = new List<DeskAsset>();
}
