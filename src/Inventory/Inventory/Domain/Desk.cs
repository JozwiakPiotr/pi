using Inventory.Api;

namespace Inventory.Domain;

public sealed class Desk
{
    private readonly List<DeskFeature> _features = [];
    private readonly List<DeskAsset> _assets = [];

    private Desk()
    {
    }

    public Desk(string number, Room room)
    {
        Id = Guid.NewGuid();
        Number = number;
        RoomId = room.Id;
        Room = room;
        State = DeskState.Available;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Number { get; private set; } = string.Empty;

    public Guid RoomId { get; private set; }

    public Room Room { get; private set; } = null!;

    public DeskState State { get; private set; }

    public IReadOnlyCollection<DeskFeature> Features => _features;

    public IReadOnlyCollection<DeskAsset> Assets => _assets;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void UpdateEquipment(IReadOnlyCollection<Feature> features, IReadOnlyCollection<(Asset Asset, int Count)> assets)
    {
        _features.Clear();
        foreach (var feature in features)
        {
            _features.Add(new DeskFeature(Id, feature.Id));
        }

        _assets.Clear();
        foreach (var (asset, count) in assets)
        {
            _assets.Add(new DeskAsset(Id, asset.Id, count));
        }

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetState(DeskState state)
    {
        State = state;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}