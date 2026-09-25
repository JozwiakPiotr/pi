namespace Inventory.Domain;

public sealed class Room
{
    private Room()
    {
    }

    public Room(string number, int level)
    {
        Id = Guid.NewGuid();
        Number = number;
        Level = level;
    }

    public Guid Id { get; private set; }

    public string Number { get; private set; } = string.Empty;

    public int Level { get; private set; }

    public ICollection<Desk> Desks { get; } = new List<Desk>();
}
