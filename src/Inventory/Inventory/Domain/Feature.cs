namespace Inventory.Domain;

public sealed class Feature
{
    private Feature()
    {
    }

    public Feature(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public ICollection<DeskFeature> Desks { get; } = new List<DeskFeature>();
}
