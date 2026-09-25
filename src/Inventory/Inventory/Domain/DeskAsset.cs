namespace Inventory.Domain;

public sealed class DeskAsset
{
    private DeskAsset()
    {
    }

    public DeskAsset(Guid deskId, Guid assetId, int count)
    {
        DeskId = deskId;
        AssetId = assetId;
        Count = count;
    }

    public Guid DeskId { get; private set; }

    public Desk Desk { get; private set; } = null!;

    public Guid AssetId { get; private set; }

    public Asset Asset { get; private set; } = null!;

    public int Count { get; private set; }
}
